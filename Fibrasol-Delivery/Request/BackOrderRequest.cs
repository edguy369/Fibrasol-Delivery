namespace Fibrasol_Delivery.Request;

public class BackOrderRequest
{
    public int OrderId { get; set; }
    public int ClientId { get; set; }
    public string Number { get; set; } = default!;
    public double Weight { get; set; } = default!;
}
