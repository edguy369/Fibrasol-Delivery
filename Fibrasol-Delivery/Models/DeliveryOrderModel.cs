namespace Fibrasol_Delivery.Models;

public class DeliveryOrderModel
{
    public int Id { get; set; }
    public DeliveryOrderStatusModel Status { get; set; } = default!;
    public DateTime Created { get; set; }
    public double Total { get; set; }
    public List<RiderModel> Riders { get; set; } = default!;
    public List<BackOrderModel> Backorders { get; set; } = default!;
}
