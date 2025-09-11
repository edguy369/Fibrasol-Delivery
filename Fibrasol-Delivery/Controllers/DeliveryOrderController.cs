using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class DeliveryOrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public DeliveryOrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
    #endregion

    #region HTTP Methods
    [HttpGet]
    [Route("delivery-orders")]
    public async Task<IActionResult> GetAllAsync()
    {
        var deliveryOrderList = await _unitOfWork.DeliveryOrders.GetAllAsync();
        return Ok(deliveryOrderList);
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
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] DeliveryOrderRequest request)
    {
        var transactionResult = await _unitOfWork.DeliveryOrders.UpdateAsync(id, request);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }
    #endregion
}
