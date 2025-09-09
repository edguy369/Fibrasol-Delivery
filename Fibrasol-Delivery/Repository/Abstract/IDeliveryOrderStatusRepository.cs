using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Request;

namespace Fibrasol_Delivery.Repository.Abstract;

public interface IDeliveryOrderStatusRepository
{
    Task<int> CreateAsync(DeliveryOrderStatusRequest request);
    Task<bool> UpdateAsync(int id, DeliveryOrderStatusRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<DeliveryOrderStatusModel>> GetAllAsync();
}
