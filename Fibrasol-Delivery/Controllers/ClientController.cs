using Fibrasol_Delivery.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class ClientController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ClientController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Views
    [Route("clientes")]
    public IActionResult Index()
    {
        return View();
    }
    #endregion
}
