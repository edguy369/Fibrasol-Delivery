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
    [HttpPost]
    [Route("delivery-orders")]
    public async Task<IActionResult> CreateAsync([FromBody] DeliveryOrderRequest request)
    {
        var transactionResult = await _unitOfWork.DeliveryOrders.CreateAsync(request);
        if (transactionResult == 0)
            return BadRequest();

        return Ok(transactionResult);
    }
    #endregion
}
