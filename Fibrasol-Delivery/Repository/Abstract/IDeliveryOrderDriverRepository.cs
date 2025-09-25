namespace Fibrasol_Delivery.Repository.Abstract
{
    public interface IDeliveryOrderDriverRepository
    {
        Task<bool> AssignAsync(int driverId, int deliveryOrderId);
        Task<bool> DeassignAsync(int driverId, int deliveryOrderId);
    }
}
