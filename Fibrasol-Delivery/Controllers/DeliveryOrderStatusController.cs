using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

public class DeliveryOrderStatusController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeliveryOrderStatusController> _logger;

    public DeliveryOrderStatusController(IUnitOfWork unitOfWork, ILogger<DeliveryOrderStatusController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [Route("estados-entrega")]
    public IActionResult Index() => View();

    [HttpGet("delivery-statuses")]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var statuses = await _unitOfWork.DeliveryOrderStatuses.GetAllAsync();
            return Ok(statuses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving delivery order statuses");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("delivery-statuses")]
    public async Task<IActionResult> CreateAsync([FromBody] DeliveryOrderStatusRequest request)
    {
        try
        {
            var result = await _unitOfWork.DeliveryOrderStatuses.CreateAsync(request);
            if (result == 0)
            {
                _logger.LogWarning("Failed to create delivery order status: {Name}", request.Name);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating delivery order status: {Name}", request.Name);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("delivery-statuses/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] DeliveryOrderStatusRequest request)
    {
        try
        {
            var result = await _unitOfWork.DeliveryOrderStatuses.UpdateAsync(id, request);
            if (!result)
            {
                _logger.LogWarning("Failed to update delivery order status ID: {Id}", id);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating delivery order status ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("delivery-statuses/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var result = await _unitOfWork.DeliveryOrderStatuses.DeleteAsync(id);
            if (!result)
            {
                _logger.LogWarning("Failed to delete delivery order status ID: {Id}", id);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting delivery order status ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
