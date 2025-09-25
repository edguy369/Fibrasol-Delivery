using Fibrasol_Delivery.Repository.Abstract;

namespace Fibrasol_Delivery.Repository;

public class UnitOfWork : IUnitOfWork
{
    public IClientRepository Clients { get; }
    public IRiderRepository Riders { get; }
    public IDeliveryOrderStatusRepository DeliveryOrderStatuses { get; }
    public IInvoiceRepository Invoices { get; }
    public IBackOrderRepository BackOrders { get; }
    public IDeliveryOrderRepository DeliveryOrders { get; }
    public IDeliveryOrderDriverRepository DeliveryOrderDrivers { get; }
    public UnitOfWork(IClientRepository clients,
        IRiderRepository riders, IDeliveryOrderStatusRepository deliveryOrderStatuses,
        IInvoiceRepository invoices, IBackOrderRepository backOrders, 
        IDeliveryOrderRepository deliveryOrders, IDeliveryOrderDriverRepository deliveryOrderDrivers)
    {
        Clients = clients;
        Riders = riders;
        DeliveryOrderStatuses = deliveryOrderStatuses;
        Invoices = invoices;
        BackOrders = backOrders;
        DeliveryOrders = deliveryOrders;
        DeliveryOrderDrivers = deliveryOrderDrivers;
    }
}
