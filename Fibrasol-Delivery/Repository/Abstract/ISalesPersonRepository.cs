using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Request;

namespace Fibrasol_Delivery.Repository.Abstract;

public interface ISalesPersonRepository
{
    Task<int> CreateAsync(SalesPersonRequest request);
    Task<bool> UpdateAsync(int id, SalesPersonRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SalesPersonModel>> GetAllAsync();
    Task<SalesPersonModel> GetByName(string name);
    Task<int> CountAsync();
}