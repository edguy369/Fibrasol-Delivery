using Fibrasol_Delivery.Repository.Abstract;

namespace Fibrasol_Delivery.Repository;

public class UnitOfWork : IUnitOfWork
{
    public IClientRepository Clients { get; }
    public IRiderRepository Riders { get; }
    public UnitOfWork(IClientRepository clients,
        IRiderRepository riders)
    {
        Clients = clients;
        Riders = riders;
    }
}
