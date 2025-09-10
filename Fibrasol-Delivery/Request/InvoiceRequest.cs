namespace Fibrasol_Delivery.Request;

public class InvoiceRequest
{
    public int BackorderId { get; set; }
    public string Address { get; set; } = default!;
    public string Reference { get; set; } = default!;
    public double Value { get; set; } = default!;
    public string? Attatchment { get; set; }
    public string? SignedAttatchment { get; set; }
}
