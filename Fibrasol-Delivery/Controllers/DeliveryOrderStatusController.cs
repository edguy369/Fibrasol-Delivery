using Fibrasol_Delivery.Repository.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

public class DeliveryOrderStatusController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public DeliveryOrderStatusController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
}
