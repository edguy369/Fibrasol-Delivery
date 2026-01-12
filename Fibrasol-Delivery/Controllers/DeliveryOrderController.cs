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
                                _logger.LogInformation("Creating new invoice for backorder {BackOrderId}", backOrderId);

                                if (invoice.SalesPersonId <= 0)
                                    return BadRequest($"SalesPersonId is required for invoice in backorder {backOrder.Id}");

                                if (!string.IsNullOrEmpty(invoice.Attatchment))
                                {
                                    var uploadResult = await _doSpaces.UploadFileAsync(invoice.Attatchment, "facturas");
                                    if (uploadResult.Success)
                                        invoice.Attatchment = uploadResult.Path;
                                }

                                if (!string.IsNullOrEmpty(invoice.SignedAttatchment))
                                {
                                    var uploadResult = await _doSpaces.UploadFileAsync(invoice.SignedAttatchment, "facturas-firmadas");
                                    if (uploadResult.Success)
                                        invoice.SignedAttatchment = uploadResult.Path;
                                }
                                invoice.BackorderId = backOrderId;

                                _logger.LogInformation("Invoice data: BackorderId={BackorderId}, ClientId={ClientId}, SalesPersonId={SalesPersonId}, Address={Address}",
                                    invoice.BackorderId, invoice.ClientId, invoice.SalesPersonId, invoice.Address);

                                try
                                {
                                    _ = await _unitOfWork.Invoices.CreateAsync(invoice);
                                    _logger.LogInformation("Invoice created successfully");
                                }
                                catch (Exception invoiceEx)
                                {
                                    _logger.LogError(invoiceEx, "Error creating invoice");
                                    return BadRequest($"Error creando factura: {invoiceEx.Message} | Inner: {invoiceEx.InnerException?.Message}");
                                }
                            }

                            if (invoice.ObjectStatus == "update")
                            {
                                _logger.LogInformation("Updating invoice {InvoiceId}", invoice.Id);

                                if (invoice.SalesPersonId <= 0)
                                    return BadRequest($"SalesPersonId is required for invoice {invoice.Id}");

                                if (invoice.AttatchmentChanged && !string.IsNullOrEmpty(invoice.Attatchment))
                                {
                                    var uploadResult = await _doSpaces.UploadFileAsync(invoice.Attatchment, "facturas");
                                    if (uploadResult.Success)
                                        invoice.Attatchment = uploadResult.Path;
                                }

                                if (invoice.signedAttatchmentChanged && !string.IsNullOrEmpty(invoice.SignedAttatchment))
                                {
                                    var uploadResult = await _doSpaces.UploadFileAsync(invoice.SignedAttatchment, "facturas-firmadas");
                                    if (uploadResult.Success)
                                        invoice.SignedAttatchment = uploadResult.Path;
                                }

                                _logger.LogInformation("Invoice update data: Id={Id}, SalesPersonId={SalesPersonId}, Address={Address}",
                                    invoice.Id, invoice.SalesPersonId, invoice.Address);

                                _ = await _unitOfWork.Invoices.UpdateAsync(invoice.Id, invoice);
                                _logger.LogInformation("Invoice {InvoiceId} updated successfully", invoice.Id);
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
            _logger.LogError(ex, "Error updating delivery order ID: {Id}", id);
            var fullError = $"ERROR_DETALLADO: {ex.Message}";
            if (ex.InnerException != null)
                fullError += $" | INNER: {ex.InnerException.Message}";
            return StatusCode(500, fullError);
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
