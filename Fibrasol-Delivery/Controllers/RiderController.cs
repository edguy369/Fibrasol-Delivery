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

    [Route("conductores")]
    public IActionResult Index() => View();

    [HttpGet("riders")]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var riders = await _unitOfWork.Riders.GetAllAsync();
            return Ok(riders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving riders");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("riders")]
    public async Task<IActionResult> CreateAsync([FromBody] RiderRequest request)
    {
        try
        {
            var result = await _unitOfWork.Riders.CreateAsync(request);
            if (result == 0)
            {
                _logger.LogWarning("Failed to create rider: {Name}", request.Name);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rider: {Name}", request.Name);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("riders/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] RiderRequest request)
    {
        try
        {
            var result = await _unitOfWork.Riders.UpdateAsync(id, request);
            if (!result)
            {
                _logger.LogWarning("Failed to update rider ID: {Id}", id);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rider ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("riders/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var result = await _unitOfWork.Riders.DeleteAsync(id);
            if (!result)
            {
                _logger.LogWarning("Failed to delete rider ID: {Id}", id);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting rider ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
