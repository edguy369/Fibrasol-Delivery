using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Request;

namespace Fibrasol_Delivery.Repository.Abstract;

public interface IDeliveryOrderRepository
{
    Task<IEnumerable<DeliveryOrderModel>> GetAllAsync();
    Task<IEnumerable<DeliveryOrderModel>> GetAllUnsignedAsync();
    Task<DeliveryOrderModel> GetByIdAsync(int id);
    Task<int> CreateAsync(DeliveryOrderRequest request);
    Task<bool> UpdateAsync(int id, DeliveryOrderCompleteRequest request);
    Task<bool> DeleteAsync(int id);
    Task<int> CountAsync();
}
