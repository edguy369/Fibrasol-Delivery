using Fibrasol_Delivery.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    #region Views
    public IActionResult Index()
    {
        return View();
    }

    #endregion

    #region Methods
    [HttpGet]
    [Route("dashboards/clients")]
    public async Task<IActionResult> CountClientsAsync()
    {
        var clientCount = await _unitOfWork.Clients.CountAsync();
        return Ok(clientCount);
    }

    [HttpGet]
    [Route("dashboards/riders")]
    public async Task<IActionResult> CountRidersAsync()
    {
        var riderCount = await _unitOfWork.Riders.CountAsync();
        return Ok(riderCount);
    }

    [HttpGet]
    [Route("dashboards/invoices")]
    public async Task<IActionResult> CountInvoicesAsync()
    {
        var invoiceCount = await _unitOfWork.Invoices.CountAsync();
        var signedInvoicesCount = await _unitOfWork.Invoices.CountSignedAsync();

        return Ok(new
        {
            Invoices = invoiceCount,
            SignedInvoices = signedInvoicesCount
        });
    }
    #endregion
}
