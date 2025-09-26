using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Request;

namespace Fibrasol_Delivery.Repository.Abstract;

public interface IClientRepository
{
    Task<int> CreateAsync(ClientRequest request);
    Task<bool> UpdateAsync(int id, ClientRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<ClientModel>> GetAllAsync();
    Task<ClientModel> GetByName(string name);
    Task<int> CountAsync();
}
