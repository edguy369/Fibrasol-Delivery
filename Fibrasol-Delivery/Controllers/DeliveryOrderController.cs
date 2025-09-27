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
    public DeliveryOrderController(IUnitOfWork unitOfWork, IDoSpaces doSpaces)
    {
        _unitOfWork = unitOfWork;
        _doSpaces = doSpaces;
    }

    #region Views
    [Route("constancias")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("constancias/{id}")]
    public IActionResult Detail(int id)
    {
        return View();
    }

    [Route("constancias/{id}/impresion")]
    public IActionResult Print(int id)
    {
        return View();
    }
    #endregion

    #region HTTP Methods
    [HttpGet]
    [Route("delivery-orders")]
    public async Task<IActionResult> GetAllAsync()
    {
        var deliveryOrderList = await _unitOfWork.DeliveryOrders.GetAllAsync();
        return Ok(deliveryOrderList);
    }

    [HttpGet]
    [Route("delivery-orders/unsigned")]
    public async Task<IActionResult> GetAllUnsignedAsync()
    {
        var deliveryOrderList = await _unitOfWork.DeliveryOrders.GetAllUnsignedAsync();
        return Ok(deliveryOrderList);
    }

    [HttpGet]
    [Route("delivery-orders/{id}")]
    public async Task<IActionResult> GetAsyncById(int id)
    {
        if (id == 0)
        {
            return Ok(null);
        }
        
        var deliveryOrder = await _unitOfWork.DeliveryOrders.GetByIdAsync(id);
        return Ok(deliveryOrder);
    }

    [HttpPost]
    [Route("delivery-orders")]
    public async Task<IActionResult> CreateAsync([FromBody] DeliveryOrderRequest request)
    {
        var transactionResult = await _unitOfWork.DeliveryOrders.CreateAsync(request);
        if (transactionResult == 0)
            return BadRequest();

        return Ok(transactionResult);
    }

    [HttpPut]
    [Route("delivery-orders/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] DeliveryOrderCompleteRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest("Request cannot be null");

            if (id <= 0)
                return BadRequest("Invalid delivery order ID");

            var transactionResult = await _unitOfWork.DeliveryOrders.UpdateAsync(id, request);
            if (!transactionResult)
                return BadRequest("Failed to update delivery order");

        //AGREGAR DRIVERS
        if (request.Riders != null && request.Riders.Count > 0)
        {
            foreach (var rider in request.Riders)
            {
                if(rider.ObjectState == "new")
                    await _unitOfWork.DeliveryOrderDrivers.AssignAsync(rider.RiderId, id);
                else if (rider.ObjectState == "delete")
                    await _unitOfWork.DeliveryOrderDrivers.DeassignAsync(rider.RiderId, id);
            }
        }

        //AGREGAR COMMANDAS
        if(request.BackOrders != null && request.BackOrders.Count > 0)
        {
            foreach (var backOrder in request.BackOrders)
            {
                int backOrderId = 0;
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

                            if (!String.IsNullOrEmpty(invoice.Attatchment))
                            {
                                var uploadImageResult = await _doSpaces.UploadFileAsync(invoice.Attatchment, "facturas");
                                if (uploadImageResult.Success)
                                    invoice.Attatchment = uploadImageResult.Path;
                            }

                            if (!String.IsNullOrEmpty(invoice.SignedAttatchment))
                            {
                                var uploadImageResult = await _doSpaces.UploadFileAsync(invoice.SignedAttatchment, "facturas-firmadas");
                                if (uploadImageResult.Success)
                                    invoice.SignedAttatchment = uploadImageResult.Path;
                            }
                            invoice.BackorderId = backOrderId;
                            _ = await _unitOfWork.Invoices.CreateAsync(invoice);
                        }

                        if (invoice.ObjectStatus == "update")
                        {
                            // Validate SalesPersonId for updated invoices
                            if (invoice.SalesPersonId <= 0)
                                return BadRequest($"SalesPersonId is required for invoice {invoice.Id}");

                            if (invoice.AttatchmentChanged)
                            {
                                if (!String.IsNullOrEmpty(invoice.Attatchment))
                                {
                                    var uploadImageResult = await _doSpaces.UploadFileAsync(invoice.Attatchment, "facturas");
                                    if (uploadImageResult.Success)
                                        invoice.Attatchment = uploadImageResult.Path;
                                }
                            }

                            if (invoice.signedAttatchmentChanged)
                            {
                                if (!String.IsNullOrEmpty(invoice.SignedAttatchment))
                                {
                                    var uploadImageResult = await _doSpaces.UploadFileAsync(invoice.SignedAttatchment, "facturas-firmadas");
                                    if (uploadImageResult.Success)
                                        invoice.SignedAttatchment = uploadImageResult.Path;
                                }
                            }

                            _ = await _unitOfWork.Invoices.UpdateAsync(invoice.Id, invoice);
                        }

                        if (invoice.ObjectStatus == "delete")
                        {
                            _ = await _unitOfWork.Invoices.DeleteAsync(invoice.Id);
                            if (!String.IsNullOrEmpty(invoice.Attatchment))
                                await _doSpaces.DeleteFileAsync(invoice.Attatchment);

                            if (!String.IsNullOrEmpty(invoice.SignedAttatchment))
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

    [HttpDelete]
    [Route("delivery-orders/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var transactionResult = await _unitOfWork.DeliveryOrders.DeleteAsync(id);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }
    #endregion
}
