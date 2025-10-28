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

    #region Views
    [Route("clientes")]
    public IActionResult Index()
    {
        return View();
    }
    #endregion

    #region Methods
    [HttpGet]
    [Route("clients")]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all clients");
            var clientList = await _unitOfWork.Clients.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {Count} clients", clientList.Count());
            return Ok(clientList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all clients");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    [Route("clients")]
    public async Task<IActionResult> CreateAsync([FromBody] ClientRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new client with name: {Name}", request.Name);
            var transactionResult = await _unitOfWork.Clients.CreateAsync(request);

            if (transactionResult == 0)
            {
                _logger.LogWarning("Failed to create client: {Name}", request.Name);
                return BadRequest();
            }

            _logger.LogInformation("Successfully created client with ID: {Id}", transactionResult);
            return Ok(transactionResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating client: {Name}", request.Name);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut]
    [Route("clients/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ClientRequest request)
    {
        try
        {
            _logger.LogInformation("Updating client with ID: {Id}", id);
            var transactionResult = await _unitOfWork.Clients.UpdateAsync(id, request);

            if (!transactionResult)
            {
                _logger.LogWarning("Failed to update client with ID: {Id}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully updated client with ID: {Id}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating client with ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete]
    [Route("clients/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting client with ID: {Id}", id);
            var transactionResult = await _unitOfWork.Clients.DeleteAsync(id);

            if (!transactionResult)
            {
                _logger.LogWarning("Failed to delete client with ID: {Id}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully deleted client with ID: {Id}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting client with ID: {Id}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    #endregion
}
