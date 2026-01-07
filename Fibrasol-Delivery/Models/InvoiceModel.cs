namespace Fibrasol_Delivery.Models;

public class InvoiceModel
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public ClientModel? Client { get; set; }
    public string Address { get; set; } = default!;
    public string Reference { get; set; } = default!;
    public double Value { get; set; } = default!;
    public string? Attatchment { get; set; }
    public string? SignedAttatchment { get; set; }
    public string Currency { get; set; } = "Q";
    public SalesPersonModel SalesPerson { get; set; } = default!;
}
