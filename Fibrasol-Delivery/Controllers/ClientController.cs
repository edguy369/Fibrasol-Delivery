using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class ClientController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientController> _logger;

    public ClientController(IUnitOfWork unitOfWork, ILogger<ClientController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [Route("clientes")]
    public IActionResult Index() => View();

    [HttpGet("clients")]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            return Ok(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving clients");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("clients")]
    public async Task<IActionResult> CreateAsync([FromBody] ClientRequest request)
    {
        try
        {
            var result = await _unitOfWork.Clients.CreateAsync(request);
            if (result == 0)
            {
                _logger.LogWarning("Failed to create client: {Name}", request.Name);
                return BadRequest();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client: {Name}", request.Name);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("clients/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ClientRequest request)
    {
        try
        {
            var result = await _unitOfWork.Clients.UpdateAsync(id, request);
            if (!result)
            {
                _logger.LogWarning("Failed to update client ID: {Id}", id);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("clients/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var result = await _unitOfWork.Clients.DeleteAsync(id);
            if (!result)
            {
                _logger.LogWarning("Failed to delete client ID: {Id}", id);
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
