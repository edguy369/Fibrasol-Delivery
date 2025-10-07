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

    #region Views
    [Route("vendedores")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("vendedores/reportes")]
    public IActionResult Report()
    {
        return View();
    }
    #endregion

    #region Methods
    [HttpGet]
    [Route("sales-persons")]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all sales persons");
            var salesPersonList = await _unitOfWork.SalesPersons.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {Count} sales persons", salesPersonList.Count());
            return Ok(salesPersonList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all sales persons");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    [Route("sales-persons")]
    public async Task<IActionResult> CreateAsync([FromBody] SalesPersonRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new sales person with name: {Name}", request.Name);
            var transactionResult = await _unitOfWork.SalesPersons.CreateAsync(request);

            if (transactionResult == 0)
            {
                _logger.LogWarning("Failed to create sales person: {Name}", request.Name);
                return BadRequest();
            }

            _logger.LogInformation("Successfully created sales person with ID: {Id}", transactionResult);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating sales person: {Name}", request.Name);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut]
    [Route("sales-persons/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SalesPersonRequest request)
    {
        try
        {
            _logger.LogInformation("Updating sales person with ID: {Id}", id);
            var transactionResult = await _unitOfWork.SalesPersons.UpdateAsync(id, request);

            if (!transactionResult)
            {
                _logger.LogWarning("Failed to update sales person with ID: {Id}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully updated sales person with ID: {Id}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating sales person with ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete]
    [Route("sales-persons/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting sales person with ID: {Id}", id);
            var transactionResult = await _unitOfWork.SalesPersons.DeleteAsync(id);

            if (!transactionResult)
            {
                _logger.LogWarning("Failed to delete sales person with ID: {Id}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully deleted sales person with ID: {Id}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting sales person with ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    [Route("sales-persons/report")]
    public async Task<IActionResult> GetReportAsync([FromBody] SalesReportRequest? request = null)
    {
        try
        {
            // If no request is provided, create one for the current month
            if (request == null)
            {
                var now = DateTime.Now;
                request = new SalesReportRequest
                {
                    StartDate = new DateTime(now.Year, now.Month, 1),
                    EndDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month))
                };
                _logger.LogInformation("No date range provided, using current month: {Start} to {End}", request.StartDate, request.EndDate);
            }

            // Validate the date range
            if (request.StartDate > request.EndDate)
            {
                _logger.LogWarning("Invalid date range: StartDate {Start} is greater than EndDate {End}", request.StartDate, request.EndDate);
                return BadRequest("Start date cannot be greater than end date.");
            }

            _logger.LogInformation("Generating sales report from {Start} to {End}", request.StartDate, request.EndDate);
            var salesReport = await _unitOfWork.SalesPersons.GetSalesReportAsync(request.StartDate, request.EndDate);
            _logger.LogInformation("Successfully generated sales report with {Count} entries", salesReport.Count());

            return Ok(salesReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating sales report");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    #endregion
}
