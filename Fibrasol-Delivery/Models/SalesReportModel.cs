namespace Fibrasol_Delivery.Models;

public class SalesReportModel
{
    public SalesPersonModel SalesPerson { get; set; } = default!;
    public double TotalSales { get; set; }
}
