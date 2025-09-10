using Fibrasol_Delivery.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

[Authorize]
public class DeliveryOrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public DeliveryOrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Views
    [Route("constancias")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("constancias/{id}")]
    public IActionResult Detail(int id)
    {
        return View();
    }
    #endregion
}
