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
        try
        {
            _logger.LogInformation("Counting riders for dashboard");
            var riderCount = await _unitOfWork.Riders.CountAsync();
            _logger.LogInformation("Successfully counted {Count} riders", riderCount);
            return Ok(riderCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while counting riders");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet]
    [Route("dashboards/delivery-orders")]
    public async Task<IActionResult> CountDeliveryOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Counting delivery orders for dashboard");
            var deliveryOrderCount = await _unitOfWork.DeliveryOrders.CountAsync();
            _logger.LogInformation("Successfully counted {Count} delivery orders", deliveryOrderCount);
            return Ok(deliveryOrderCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while counting delivery orders");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet]
    [Route("dashboards/invoices")]
    public async Task<IActionResult> CountInvoicesAsync()
    {
        try
        {
            _logger.LogInformation("Counting invoices for dashboard");
            var invoiceCount = await _unitOfWork.Invoices.CountAsync();
            var signedInvoicesCount = await _unitOfWork.Invoices.CountSignedAsync();

            _logger.LogInformation("Successfully counted {Total} total invoices and {Signed} signed invoices", invoiceCount, signedInvoicesCount);
            return Ok(new
            {
                Invoices = invoiceCount,
                SignedInvoices = signedInvoicesCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while counting invoices");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet]
    [Route("dashboards/sales-persons")]
    public async Task<IActionResult> CountSalesPersonsAsync()
    {
        try
        {
            _logger.LogInformation("Counting sales persons for dashboard");
            var salesPersonCount = await _unitOfWork.SalesPersons.CountAsync();
            _logger.LogInformation("Successfully counted {Count} sales persons", salesPersonCount);
            return Ok(salesPersonCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while counting sales persons");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    #endregion
}
