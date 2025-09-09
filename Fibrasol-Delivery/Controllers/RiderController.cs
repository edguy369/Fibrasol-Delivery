using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

public class RiderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public RiderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Views
    [Route("conductores")]
    public IActionResult Index()
    {
        return View();
    }
    #endregion

    #region Methods
    [HttpGet]
    [Route("riders")]
    public async Task<IActionResult> GetAllAsync()
    {
        var riderList = await _unitOfWork.Riders.GetAllAsync();
        return Ok(riderList);
    }

    [HttpPost]
    [Route("riders")]
    public async Task<IActionResult> CreateAsync([FromBody] RiderRequest request)
    {
        var transactionResult = await _unitOfWork.Riders.CreateAsync(request);
        if (transactionResult == 0)
            return BadRequest();

        return Ok();
    }

    [HttpPut]
    [Route("riders/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] RiderRequest request)
    {
        var transactionResult = await _unitOfWork.Riders.UpdateAsync(id, request);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }

    [HttpDelete]
    [Route("riders/{id}")]
    public async Task<IActionResult> DeleteAsync(int id, [FromBody] RiderRequest request)
    {
        var transactionResult = await _unitOfWork.Riders.DeleteAsync(id);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }
    #endregion
}
