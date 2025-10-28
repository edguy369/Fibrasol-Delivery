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

    #region Views
    [Route("estados-entrega")]
    public IActionResult Index()
    {
        return View();
    }
    #endregion

    #region Methods
    [HttpGet]
    [Route("delivery-statuses")]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all delivery order statuses");
            var deliveryOrderStatuses = await _unitOfWork.DeliveryOrderStatuses.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {Count} delivery order statuses", deliveryOrderStatuses.Count());
            return Ok(deliveryOrderStatuses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all delivery order statuses");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    [Route("delivery-statuses")]
    public async Task<IActionResult> CreateAsync([FromBody] DeliveryOrderStatusRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new delivery order status with name: {Name}", request.Name);
            var transactionResult = await _unitOfWork.DeliveryOrderStatuses.CreateAsync(request);

            if (transactionResult == 0)
            {
                _logger.LogWarning("Failed to create delivery order status: {Name}", request.Name);
                return BadRequest();
            }

            _logger.LogInformation("Successfully created delivery order status with ID: {Id}", transactionResult);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating delivery order status: {Name}", request.Name);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut]
    [Route("delivery-statuses/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] DeliveryOrderStatusRequest request)
    {
        try
        {
            _logger.LogInformation("Updating delivery order status with ID: {Id}", id);
            var transactionResult = await _unitOfWork.DeliveryOrderStatuses.UpdateAsync(id, request);

            if (!transactionResult)
            {
                _logger.LogWarning("Failed to update delivery order status with ID: {Id}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully updated delivery order status with ID: {Id}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating delivery order status with ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete]
    [Route("delivery-statuses/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting delivery order status with ID: {Id}", id);
            var transactionResult = await _unitOfWork.DeliveryOrderStatuses.DeleteAsync(id);

            if (!transactionResult)
            {
                _logger.LogWarning("Failed to delete delivery order status with ID: {Id}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully deleted delivery order status with ID: {Id}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting delivery order status with ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    #endregion
}
