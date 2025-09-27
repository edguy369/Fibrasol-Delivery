namespace Fibrasol_Delivery.Request;

public class InvoiceRequest
{
    public int Id { get; set; }
    public int BackorderId { get; set; }
    public string Address { get; set; } = default!;
    public string Reference { get; set; } = default!;
    public double Value { get; set; } = default!;
    public string? Attatchment { get; set; }
    public string? SignedAttatchment { get; set; }
    public int SalesPersonId { get; set; }
    public string ObjectStatus { get; set; } = default!;
    public bool AttatchmentChanged { get; set; } = default!;
    public bool signedAttatchmentChanged { get; set; } = default!;
}
