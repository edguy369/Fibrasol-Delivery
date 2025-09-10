using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Request;

namespace Fibrasol_Delivery.Repository.Abstract;

public interface IDeliveryOrderRepository
{
    Task<IEnumerable<DeliveryOrderModel>> GetAllAsync();
    Task<int> CreateAsync(DeliveryOrderRequest request);
    Task<bool> UpdateAsync(int id, DeliveryOrderRequest request);
    Task<bool> DeleteAsync(int id);
}
