namespace Fibrasol_Delivery.Repository.Abstract;

public interface IUnitOfWork
{
    public IClientRepository Clients { get; }
    public IRiderRepository Riders { get; }
    public IDeliveryOrderStatusRepository DeliveryOrderStatuses { get; }
    public IInvoiceRepository Invoices { get; }
    public IBackOrderRepository BackOrders { get; }
    public IDeliveryOrderRepository DeliveryOrders { get; }
    public IDeliveryOrderDriverRepository DeliveryOrderDrivers { get; }
    public ISalesPersonRepository SalesPersons { get; }
}
