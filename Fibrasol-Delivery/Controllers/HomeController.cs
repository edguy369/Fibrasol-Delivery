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
    public IActionResult Index() => View();

    [HttpGet("dashboards/clients")]
    public async Task<IActionResult> CountClientsAsync()
    {
        try
        {
            var count = await _unitOfWork.Clients.CountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting clients");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("dashboards/riders")]
    public async Task<IActionResult> CountRidersAsync()
    {
        try
        {
            var count = await _unitOfWork.Riders.CountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting riders");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("dashboards/delivery-orders")]
    public async Task<IActionResult> CountDeliveryOrdersAsync()
    {
        try
        {
            var count = await _unitOfWork.DeliveryOrders.CountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting delivery orders");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("dashboards/invoices")]
    public async Task<IActionResult> CountInvoicesAsync()
    {
        try
        {
            var invoiceCount = await _unitOfWork.Invoices.CountAsync();
            var signedInvoicesCount = await _unitOfWork.Invoices.CountSignedAsync();

            return Ok(new
            {
                Invoices = invoiceCount,
                SignedInvoices = signedInvoicesCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting invoices");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("dashboards/sales-persons")]
    public async Task<IActionResult> CountSalesPersonsAsync()
    {
        try
        {
            var count = await _unitOfWork.SalesPersons.CountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting sales persons");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
