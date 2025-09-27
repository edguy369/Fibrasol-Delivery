using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class SalesPersonController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public SalesPersonController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Views
    [Route("vendedores")]
    public IActionResult Index()
    {
        return View();
    }
    #endregion

    #region Methods
    [HttpGet]
    [Route("sales-persons")]
    public async Task<IActionResult> GetAllAsync()
    {
        var salesPersonList = await _unitOfWork.SalesPersons.GetAllAsync();
        return Ok(salesPersonList);
    }

    [HttpPost]
    [Route("sales-persons")]
    public async Task<IActionResult> CreateAsync([FromBody] SalesPersonRequest request)
    {
        var transactionResult = await _unitOfWork.SalesPersons.CreateAsync(request);
        if (transactionResult == 0)
            return BadRequest();

        return Ok();
    }

    [HttpPut]
    [Route("sales-persons/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SalesPersonRequest request)
    {
        var transactionResult = await _unitOfWork.SalesPersons.UpdateAsync(id, request);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }

    [HttpDelete]
    [Route("sales-persons/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var transactionResult = await _unitOfWork.SalesPersons.DeleteAsync(id);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }
    #endregion
}
