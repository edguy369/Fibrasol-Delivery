using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

public class RiderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RiderController> _logger;

    public RiderController(IUnitOfWork unitOfWork, ILogger<RiderController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
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
        try
        {
            _logger.LogInformation("Retrieving all riders");
            var riderList = await _unitOfWork.Riders.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {Count} riders", riderList.Count());
            return Ok(riderList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all riders");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    [Route("riders")]
    public async Task<IActionResult> CreateAsync([FromBody] RiderRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new rider with name: {Name}", request.Name);
            var transactionResult = await _unitOfWork.Riders.CreateAsync(request);

            if (transactionResult == 0)
            {
                _logger.LogWarning("Failed to create rider: {Name}", request.Name);
                return BadRequest();
            }

            _logger.LogInformation("Successfully created rider with ID: {Id}", transactionResult);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating rider: {Name}", request.Name);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut]
    [Route("riders/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] RiderRequest request)
    {
        try
        {
            _logger.LogInformation("Updating rider with ID: {Id}", id);
            var transactionResult = await _unitOfWork.Riders.UpdateAsync(id, request);

            if (!transactionResult)
            {
                _logger.LogWarning("Failed to update rider with ID: {Id}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully updated rider with ID: {Id}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating rider with ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete]
    [Route("riders/{id}")]
    public async Task<IActionResult> DeleteAsync(int id, [FromBody] RiderRequest request)
    {
        try
        {
            _logger.LogInformation("Deleting rider with ID: {Id}", id);
            var transactionResult = await _unitOfWork.Riders.DeleteAsync(id);

            if (!transactionResult)
            {
                _logger.LogWarning("Failed to delete rider with ID: {Id}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully deleted rider with ID: {Id}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting rider with ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    #endregion
}
