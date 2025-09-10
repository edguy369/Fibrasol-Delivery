using Fibrasol_Delivery.Request;

namespace Fibrasol_Delivery.Repository.Abstract;

public interface IInvoiceRepository
{
    Task<int> CreateAsync(InvoiceRequest request);
    Task<bool> UpdateAsync(int id, InvoiceRequest request);
    Task<bool> DeleteAsync(int id);
}
