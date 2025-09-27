namespace Fibrasol_Delivery.Models;

public class BackOrderModel
{
    public int Id { get; set; }
    public SalesPersonModel Client { get; set; } = default!;
    public string Number { get; set; } = default!;
    public double Weight { get; set; } = default!;
    public List<InvoiceModel> Invoices { get; set; } = default!;
}
