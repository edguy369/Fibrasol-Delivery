using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

public class DeliveryOrderStatusController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public DeliveryOrderStatusController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Views
    [Route("estados-entrega")]
    public IActionResult Index()
    {
        return View();
    }
    #endregion

    #region Methods
    [HttpGet]
    [Route("delivery-statuses")]
    public async Task<IActionResult> GetAllAsync()
    {
        var deliveryOrderStatuses = await _unitOfWork.DeliveryOrderStatuses.GetAllAsync();
        return Ok(deliveryOrderStatuses);
    }

    [HttpPost]
    [Route("delivery-statuses")]
    public async Task<IActionResult> CreateAsync([FromBody] DeliveryOrderStatusRequest request)
    {
        var transactionResult = await _unitOfWork.DeliveryOrderStatuses.CreateAsync(request);
        if (transactionResult == 0)
            return BadRequest();

        return Ok();
    }

    [HttpPut]
    [Route("delivery-statuses/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] DeliveryOrderStatusRequest request)
    {
        var transactionResult = await _unitOfWork.DeliveryOrderStatuses.UpdateAsync(id, request);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }

    [HttpDelete]
    [Route("delivery-statuses/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var transactionResult = await _unitOfWork.DeliveryOrderStatuses.DeleteAsync(id);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }
    #endregion
}
