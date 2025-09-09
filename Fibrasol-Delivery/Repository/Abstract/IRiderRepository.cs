using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Request;

namespace Fibrasol_Delivery.Repository.Abstract;

public interface IRiderRepository
{
    Task<int> CreateAsync(RiderRequest request);
    Task<bool> UpdateAsync(int id, RiderRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<RiderModel>> GetAllAsync();
}
