namespace Fibrasol_Delivery.Request;

public class DeliveryOrderCompleteRequest
{
    public double Total { get; set; }
    public int StatusId { get; set; }
    public List<BackOrderRequest> BackOrders { get; set; } = default!;
}
