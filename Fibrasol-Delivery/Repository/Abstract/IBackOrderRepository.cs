using Fibrasol_Delivery.Request;

namespace Fibrasol_Delivery.Repository.Abstract;

public interface IBackOrderRepository
{
    Task<int> CreateAsync(BackOrderRequest request);
    Task<bool> UpdateAsync(int id, BackOrderRequest request);
    Task<bool> DeleteAsync(int id);
}
