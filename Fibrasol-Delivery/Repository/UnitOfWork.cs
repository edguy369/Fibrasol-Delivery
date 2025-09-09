using Fibrasol_Delivery.Repository.Abstract;

namespace Fibrasol_Delivery.Repository;

public class UnitOfWork : IUnitOfWork
{
    public IClientRepository Clients { get; }
    public IRiderRepository Riders { get; }
    public IDeliveryOrderStatusRepository DeliveryOrderStatuses { get; }
    public UnitOfWork(IClientRepository clients,
        IRiderRepository riders, IDeliveryOrderStatusRepository deliveryOrderStatuses)
    {
        Clients = clients;
        Riders = riders;
        DeliveryOrderStatuses = deliveryOrderStatuses;
    }
}
