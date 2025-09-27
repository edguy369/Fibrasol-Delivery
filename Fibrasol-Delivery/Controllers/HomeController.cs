using Fibrasol_Delivery.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
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
        try
        {
            var clientCount = await _unitOfWork.Clients.CountAsync();
            return Ok(clientCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while counting clients.");
            throw;
        }
    }

    [HttpGet]
    [Route("dashboards/riders")]
    public async Task<IActionResult> CountRidersAsync()
    {
        var riderCount = await _unitOfWork.Riders.CountAsync();
        return Ok(riderCount);
    }

    [HttpGet]
    [Route("dashboards/delivery-orders")]
    public async Task<IActionResult> CountDeliveryOrdersAsync()
    {
        var deliveryOrderCount = await _unitOfWork.DeliveryOrders.CountAsync();
        return Ok(deliveryOrderCount);
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
