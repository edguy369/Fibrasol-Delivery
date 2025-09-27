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
                    StartDate = new DateTime(now.Year, now.Month, 1), // First day of current month
                    EndDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)) // Last day of current month
                };
            }

            // Validate the date range
            if (request.StartDate > request.EndDate)
            {
                return BadRequest("Start date cannot be greater than end date.");
            }

            // Call the repository method to get the sales report
            var salesReport = await _unitOfWork.SalesPersons.GetSalesReportAsync(request.StartDate, request.EndDate);

            return Ok(salesReport);
        }
        catch (Exception ex)
        {
            // Log the exception here if you have logging configured
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    #endregion
}
