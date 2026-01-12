using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class SalesPersonController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SalesPersonController> _logger;

    public SalesPersonController(IUnitOfWork unitOfWork, ILogger<SalesPersonController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [Route("vendedores")]
    public IActionResult Index() => View();

    [Route("vendedores/reportes")]
    public IActionResult Report() => View();

    [HttpGet("sales-persons")]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var salesPersons = await _unitOfWork.SalesPersons.GetAllAsync();
            return Ok(salesPersons);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales persons");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("sales-persons")]
    public async Task<IActionResult> CreateAsync([FromBody] SalesPersonRequest request)
    {
        try
        {
            var result = await _unitOfWork.SalesPersons.CreateAsync(request);
            if (result == 0)
            {
                _logger.LogWarning("Failed to create sales person: {Name}", request.Name);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sales person: {Name}", request.Name);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("sales-persons/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SalesPersonRequest request)
    {
        try
        {
            var result = await _unitOfWork.SalesPersons.UpdateAsync(id, request);
            if (!result)
            {
                _logger.LogWarning("Failed to update sales person ID: {Id}", id);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sales person ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("sales-persons/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var result = await _unitOfWork.SalesPersons.DeleteAsync(id);
            if (!result)
            {
                _logger.LogWarning("Failed to delete sales person ID: {Id}", id);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sales person ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("sales-persons/report")]
    public async Task<IActionResult> GetReportAsync([FromBody] SalesReportRequest? request = null)
    {
        try
        {
            if (request == null)
            {
                var now = DateTime.Now;
                request = new SalesReportRequest
                {
                    StartDate = new DateTime(now.Year, now.Month, 1),
                    EndDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month))
                };
            }

            if (request.StartDate > request.EndDate)
            {
                _logger.LogWarning("Invalid date range: {Start} > {End}", request.StartDate, request.EndDate);
                return BadRequest("Start date cannot be greater than end date.");
            }

            var report = await _unitOfWork.SalesPersons.GetSalesReportAsync(request.StartDate, request.EndDate);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales report");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
