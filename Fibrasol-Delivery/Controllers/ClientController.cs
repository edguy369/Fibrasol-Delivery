using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class ClientController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ClientController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Views
    [Route("clientes")]
    public IActionResult Index()
    {
        return View();
    }
    #endregion

    #region Methods
    [HttpGet]
    [Route("clients")]
    public async Task<IActionResult> GetAllAsync()
    {
        var clientList = await _unitOfWork.Clients.GetAllAsync();
        return Ok(clientList);
    }

    [HttpPost]
    [Route("clients")]
    public async Task<IActionResult> CreateAsync([FromBody] ClientRequest request)
    {
        var transactionResult = await _unitOfWork.Clients.CreateAsync(request);
        if (transactionResult == 0)
            return BadRequest();

        return Ok(transactionResult);
    }

    [HttpPut]
    [Route("clients/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ClientRequest request)
    {
        var transactionResult = await _unitOfWork.Clients.UpdateAsync(id, request);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }

    [HttpDelete]
    [Route("clients/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var transactionResult = await _unitOfWork.Clients.DeleteAsync(id);
        if (!transactionResult)
            return BadRequest();

        return Ok();
    }
    #endregion
}
