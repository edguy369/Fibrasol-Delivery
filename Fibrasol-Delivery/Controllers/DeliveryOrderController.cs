using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tipi.Tools.Services.Interfaces;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class DeliveryOrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDoSpaces _doSpaces;
    private readonly ILogger<DeliveryOrderController> _logger;

    public DeliveryOrderController(IUnitOfWork unitOfWork, IDoSpaces doSpaces, ILogger<DeliveryOrderController> logger)
    {
        _unitOfWork = unitOfWork;
        _doSpaces = doSpaces;
        _logger = logger;
    }

    [Route("constancias")]
    public IActionResult Index() => View();

    [Route("constancias/{id}")]
    public IActionResult Detail(int id) => View();

    [Route("constancias/{id}/impresion")]
    public IActionResult Print(int id) => View();

    [HttpGet("delivery-orders")]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var deliveryOrders = await _unitOfWork.DeliveryOrders.GetAllAsync();
            return Ok(deliveryOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving delivery orders");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("delivery-orders/unsigned")]
    public async Task<IActionResult> GetAllUnsignedAsync()
    {
        try
        {
            var deliveryOrders = await _unitOfWork.DeliveryOrders.GetAllUnsignedAsync();
            return Ok(deliveryOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unsigned delivery orders");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("delivery-orders/{id}")]
    public async Task<IActionResult> GetAsyncById(int id)
    {
        try
        {
            if (id == 0)
            {
                _logger.LogWarning("Attempted to retrieve delivery order with ID 0");
                return Ok(null);
            }

            var deliveryOrder = await _unitOfWork.DeliveryOrders.GetByIdAsync(id);
            return Ok(deliveryOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving delivery order ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("delivery-orders")]
    public async Task<IActionResult> CreateAsync([FromBody] DeliveryOrderRequest request)
    {
        try
        {
            var result = await _unitOfWork.DeliveryOrders.CreateAsync(request);
            if (result == 0)
            {
                _logger.LogWarning("Failed to create delivery order");
                return BadRequest();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating delivery order");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("delivery-orders/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] DeliveryOrderCompleteRequest request)
    {
        try
        {
            if (request == null)
            {
                _logger.LogWarning("Update request is null for delivery order ID: {Id}", id);
                return BadRequest("Request cannot be null");
            }

            if (id <= 0)
            {
                _logger.LogWarning("Invalid delivery order ID: {Id}", id);
                return BadRequest("Invalid delivery order ID");
            }

            var result = await _unitOfWork.DeliveryOrders.UpdateAsync(id, request);
            if (!result)
            {
                _logger.LogWarning("Failed to update delivery order ID: {Id}", id);
                return BadRequest("Failed to update delivery order");
            }

            // Process riders
            if (request.Riders != null && request.Riders.Count > 0)
            {
                foreach (var rider in request.Riders)
                {
                    if (rider.ObjectState == "new")
                        await _unitOfWork.DeliveryOrderDrivers.AssignAsync(rider.RiderId, id);
                    else if (rider.ObjectState == "delete")
                        await _unitOfWork.DeliveryOrderDrivers.DeassignAsync(rider.RiderId, id);
                }
            }

            // Process back orders
            if (request.BackOrders != null && request.BackOrders.Count > 0)
            {
                foreach (var backOrder in request.BackOrders)
                {
                    int backOrderId = backOrder.Id; // Default to existing ID for "existing" status
                    if (backOrder.ObjectStatus == "new")
                        backOrderId = await _unitOfWork.BackOrders.CreateAsync(backOrder);
                    else if (backOrder.ObjectStatus == "update")
                    {
                        _ = await _unitOfWork.BackOrders.UpdateAsync(backOrder.Id, backOrder);
                        backOrderId = backOrder.Id;
                    }
                    else if (backOrder.ObjectStatus == "delete")
                        _ = await _unitOfWork.BackOrders.DeleteAsync(backOrder.Id);

                if (backOrder.Invoices != null)
                {
                    foreach (var invoice in backOrder.Invoices)
                    {
                        if (invoice.ObjectStatus == "new")
                        {
                            // Validate SalesPersonId for new invoices
                            if (invoice.SalesPersonId <= 0)
                                return BadRequest($"SalesPersonId is required for invoice in backorder {backOrder.Id}");

                            if (!string.IsNullOrEmpty(invoice.Attatchment))
                            {
                                _logger.LogInformation("Uploading attachment for new invoice. Data length: {Length}", invoice.Attatchment.Length);
                                var uploadResult = await _doSpaces.UploadFileAsync(invoice.Attatchment, "facturas");
                                if (uploadResult.Success)
                                {
                                    _logger.LogInformation("Attachment uploaded successfully. Path: {Path}", uploadResult.Path);
                                    invoice.Attatchment = uploadResult.Path;
                                }
                                else
                                {
                                    _logger.LogError("Failed to upload attachment: {Error}", uploadResult.Error);
                                    invoice.Attatchment = null; // Clear to avoid saving base64 to DB
                                }
                            }

                            if (!string.IsNullOrEmpty(invoice.SignedAttatchment))
                            {
                                _logger.LogInformation("Uploading signed attachment for new invoice. Data length: {Length}", invoice.SignedAttatchment.Length);
                                var uploadImageResult = await _doSpaces.UploadFileAsync(invoice.SignedAttatchment, "facturas-firmadas");
                                if (uploadImageResult.Success)
                                {
                                    _logger.LogInformation("Signed attachment uploaded successfully. Path: {Path}", uploadImageResult.Path);
                                    invoice.SignedAttatchment = uploadImageResult.Path;
                                }
                                else
                                {
                                    _logger.LogError("Failed to upload signed attachment: {Error}", uploadImageResult.Error);
                                    invoice.SignedAttatchment = null; // Clear to avoid saving base64 to DB
                                }
                            }
                            invoice.BackorderId = backOrderId;
                            _ = await _unitOfWork.Invoices.CreateAsync(invoice);
                        }

                        if (invoice.ObjectStatus == "update")
                        {
                            // Validate SalesPersonId for updated invoices
                            if (invoice.SalesPersonId <= 0)
                                return BadRequest($"SalesPersonId is required for invoice {invoice.Id}");

                            if (invoice.AttatchmentChanged && !string.IsNullOrEmpty(invoice.Attatchment))
                            {
                                _logger.LogInformation("Uploading attachment for updated invoice {Id}. Data length: {Length}", invoice.Id, invoice.Attatchment.Length);
                                var uploadResult = await _doSpaces.UploadFileAsync(invoice.Attatchment, "facturas");
                                if (uploadResult.Success)
                                {
                                    _logger.LogInformation("Attachment uploaded successfully. Path: {Path}", uploadResult.Path);
                                    invoice.Attatchment = uploadResult.Path;
                                }
                                else
                                {
                                    _logger.LogError("Failed to upload attachment for invoice {Id}: {Error}", invoice.Id, uploadResult.Error);
                                    invoice.Attatchment = null; // Clear to avoid saving base64 to DB
                                }
                            }

                            if (invoice.signedAttatchmentChanged && !string.IsNullOrEmpty(invoice.SignedAttatchment))
                            {
                                _logger.LogInformation("Uploading signed attachment for updated invoice {Id}. Data length: {Length}", invoice.Id, invoice.SignedAttatchment.Length);
                                var uploadResult = await _doSpaces.UploadFileAsync(invoice.SignedAttatchment, "facturas-firmadas");
                                if (uploadResult.Success)
                                {
                                    _logger.LogInformation("Signed attachment uploaded successfully. Path: {Path}", uploadResult.Path);
                                    invoice.SignedAttatchment = uploadResult.Path;
                                }
                                else
                                {
                                    _logger.LogError("Failed to upload signed attachment for invoice {Id}: {Error}", invoice.Id, uploadResult.Error);
                                    invoice.SignedAttatchment = null; // Clear to avoid saving base64 to DB
                                }
                            }

                            _ = await _unitOfWork.Invoices.UpdateAsync(invoice.Id, invoice);
                        }

                            if (invoice.ObjectStatus == "delete")
                            {
                                _ = await _unitOfWork.Invoices.DeleteAsync(invoice.Id);
                                if (!string.IsNullOrEmpty(invoice.Attatchment))
                                    await _doSpaces.DeleteFileAsync(invoice.Attatchment);

                                if (!string.IsNullOrEmpty(invoice.SignedAttatchment))
                                    await _doSpaces.DeleteFileAsync(invoice.SignedAttatchment);
                            }
                        }
                    }
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            // Log the exception here if you have logging configured
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("delivery-orders/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var result = await _unitOfWork.DeliveryOrders.DeleteAsync(id);
            if (!result)
            {
                _logger.LogWarning("Failed to delete delivery order ID: {Id}", id);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting delivery order ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
