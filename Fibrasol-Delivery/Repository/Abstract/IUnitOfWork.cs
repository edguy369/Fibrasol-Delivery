namespace Fibrasol_Delivery.Repository.Abstract;

public interface IUnitOfWork
{
    public IClientRepository Clients { get; }
    public IRiderRepository Riders { get; }
}
