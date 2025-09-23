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
        var transactionResult = await _unitOfWork.DeliveryOrders.UpdateAsync(id, request);
        if (!transactionResult)
            return BadRequest();

        //AGREGAR COMMANDAS
        if(request.BackOrders.Count != 0)
            foreach (var backOrder in request.BackOrders)
            {
                if (backOrder.ObjectStatus == "new")
                    _ = await _unitOfWork.BackOrders.CreateAsync(backOrder);

                if (backOrder.ObjectStatus == "update")
                    _ = await _unitOfWork.BackOrders.UpdateAsync(backOrder.Id, backOrder);

                if (backOrder.ObjectStatus == "delete")
                    _ = await _unitOfWork.BackOrders.DeleteAsync(backOrder.Id);
            }
                


        return Ok();
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
