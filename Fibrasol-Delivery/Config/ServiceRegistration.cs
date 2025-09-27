using Fibrasol_Delivery.Repository;
using Fibrasol_Delivery.Repository.Abstract;

namespace Fibrasol_Delivery.Config;

public static class ServiceRegistration
{
    public static void ConfigureDataAccessLayer(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = new ConnectionString(config["ConnectionString"]);
        services.AddSingleton(connectionString);

        services.AddTransient<IRiderRepository, RiderRepository>();
        services.AddTransient<IClientRepository, ClientRepository>();
        services.AddTransient<IDeliveryOrderStatusRepository, DeliveryOrderStatusRepository>();
        services.AddTransient<IInvoiceRepository, InvoiceRepository>();
        services.AddTransient<IBackOrderRepository, BackOrderRepository>();
        services.AddTransient<IDeliveryOrderRepository, DeliveryOrderRepository>();
        services.AddTransient<IDeliveryOrderDriverRepository, DeliveryOrderDriverRepository>();
        services.AddTransient<ISalesPersonRepository, SalesPersonRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
}
