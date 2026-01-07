using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;
using System.Data;

namespace Fibrasol_Delivery.Repository;

public class DeliveryOrderRepository : IDeliveryOrderRepository
{
    private readonly string _connectionString;

    public DeliveryOrderRepository(ConnectionString connectionString)
    {
        _connectionString = connectionString.Value;
    }

    public async Task<int> CountAsync()
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_DeliveryOrder_Count",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<int> CreateAsync(DeliveryOrderRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_DeliveryOrder_Create",
            new { pStatusId = 1, pTotal = request.Total },
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_DeliveryOrder_Delete",
            new { pId = id },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }

    public async Task<IEnumerable<DeliveryOrderModel>> GetAllAsync()
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<DeliveryOrderModel, DeliveryOrderStatusModel, DeliveryOrderModel>(
            "sp_DeliveryOrder_GetAll",
            (deliveryOrder, status) =>
            {
                deliveryOrder.Status = status;
                return deliveryOrder;
            },
            commandType: CommandType.StoredProcedure,
            splitOn: "StatusId");
        return transactionResult;
    }

    public async Task<IEnumerable<DeliveryOrderModel>> GetAllUnsignedAsync()
    {
        var deliveryDisctionary = new Dictionary<int, DeliveryOrderModel>();
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<DeliveryOrderModel, DeliveryOrderStatusModel, DeliveryOrderModel>(
            "sp_DeliveryOrder_GetAllUnsigned",
            (deliveryOrder, status) =>
            {
                if (!deliveryDisctionary.TryGetValue(deliveryOrder.Id, out DeliveryOrderModel? myOrder))
                {
                    myOrder = deliveryOrder;
                    myOrder.Status = status;
                    deliveryDisctionary.Add(myOrder.Id, myOrder);
                }

                return myOrder;
            },
            commandType: CommandType.StoredProcedure,
            splitOn: "StatusId");
        return transactionResult.Distinct();
    }

    public async Task<DeliveryOrderModel> GetByIdAsync(int id)
    {
        using var conn = new MySqlConnection(_connectionString);
        var deliveryDictionary = new Dictionary<int, DeliveryOrderModel>();
        var riderDictionary = new Dictionary<int, RiderModel>();
        var backOrderDictionary = new Dictionary<int, BackOrderModel>();
        var invoiceDictionary = new Dictionary<int, InvoiceModel>();

        // Query with named columns for robust mapping
        const string sql = @"
            SELECT
                a.Id AS OrderId, a.Created, a.Total, a.StatusId,
                b.Id AS StatusId2, b.Name AS StatusName,
                f.Id AS RiderAssignationId,
                g.Id AS RiderId, g.Name AS RiderName,
                c.Id AS BackorderId, c.Number AS BackorderNumber, c.Weight AS BackorderWeight,
                d.Id AS BackOrderClientId, d.Name AS BackOrderClientName,
                e.Id AS InvoiceId, e.Address, e.Reference, e.Value, e.Attatchment, e.SignedAttatchment, e.Currency,
                ic.Id AS InvoiceClientId, ic.Name AS InvoiceClientName,
                h.Id AS SalesPersonId2, h.Name AS SalesPersonName
            FROM DeliveryOrder a
            INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id
            LEFT JOIN BackOrder c ON a.Id = c.DeliveryOrderId
            LEFT JOIN Clients d ON d.Id = c.ClientId
            LEFT JOIN Invoice e ON e.BackorderId = c.Id
            LEFT JOIN Clients ic ON ic.Id = e.ClientId
            LEFT JOIN DeliveryOrderDrivers f ON f.DeliveryOrderId = a.Id
            LEFT JOIN Drivers g ON g.Id = f.DriverId
            LEFT JOIN SalesPerson h ON h.Id = e.SalesPersonId
            WHERE a.Id = @pId";

        var transactionResult = await conn.QueryAsync(sql, new { pId = id });

        foreach (var row in transactionResult)
        {
            var dict = (IDictionary<string, object>)row;

            // Map DeliveryOrder
            int deliveryOrderId = (int)dict["OrderId"];
            if (!deliveryDictionary.TryGetValue(deliveryOrderId, out DeliveryOrderModel? myOrder))
            {
                myOrder = new DeliveryOrderModel
                {
                    Id = deliveryOrderId,
                    Created = (DateTime)dict["Created"],
                    Total = (double)dict["Total"],
                    Status = new DeliveryOrderStatusModel
                    {
                        Id = (int)dict["StatusId2"],
                        Name = (string)dict["StatusName"]
                    },
                    Backorders = new List<BackOrderModel>(),
                    Riders = new List<RiderModel>()
                };
                deliveryDictionary.Add(myOrder.Id, myOrder);
            }

            // Map Rider
            var riderId = dict["RiderId"] as int?;
            if (riderId.HasValue && riderId.Value > 0 && !riderDictionary.ContainsKey(riderId.Value))
            {
                var rider = new RiderModel
                {
                    Id = riderId.Value,
                    Name = (string)dict["RiderName"]
                };
                myOrder.Riders.Add(rider);
                riderDictionary.Add(rider.Id, rider);
            }

            // Map BackOrder
            var backorderId = dict["BackorderId"] as int?;
            if (backorderId.HasValue && backorderId.Value > 0)
            {
                if (!backOrderDictionary.TryGetValue(backorderId.Value, out BackOrderModel? myBackOrder))
                {
                    myBackOrder = new BackOrderModel
                    {
                        Id = backorderId.Value,
                        Number = (string)dict["BackorderNumber"],
                        Weight = (double)dict["BackorderWeight"],
                        Invoices = new List<InvoiceModel>()
                    };

                    // Map BackOrder Client
                    var backOrderClientId = dict["BackOrderClientId"] as int?;
                    if (backOrderClientId.HasValue && backOrderClientId.Value > 0)
                    {
                        myBackOrder.Client = new ClientModel
                        {
                            Id = backOrderClientId.Value,
                            Name = (string)dict["BackOrderClientName"]
                        };
                    }

                    myOrder.Backorders.Add(myBackOrder);
                    backOrderDictionary.Add(myBackOrder.Id, myBackOrder);
                }

                // Map Invoice
                var invoiceId = dict["InvoiceId"] as int?;
                if (invoiceId.HasValue && invoiceId.Value > 0 && !invoiceDictionary.ContainsKey(invoiceId.Value))
                {
                    var invoice = new InvoiceModel
                    {
                        Id = invoiceId.Value,
                        Address = (string)dict["Address"],
                        Reference = (string)dict["Reference"],
                        Value = (double)dict["Value"],
                        Attatchment = dict["Attatchment"] as string,
                        SignedAttatchment = dict["SignedAttatchment"] as string,
                        Currency = (string)dict["Currency"]
                    };

                    // Map Invoice Client
                    var invoiceClientId = dict["InvoiceClientId"] as int?;
                    if (invoiceClientId.HasValue && invoiceClientId.Value > 0)
                    {
                        invoice.ClientId = invoiceClientId.Value;
                        invoice.Client = new ClientModel
                        {
                            Id = invoiceClientId.Value,
                            Name = (string)dict["InvoiceClientName"]
                        };
                    }

                    // Map SalesPerson
                    var salesPersonId = dict["SalesPersonId2"] as int?;
                    if (salesPersonId.HasValue && salesPersonId.Value > 0)
                    {
                        invoice.SalesPerson = new SalesPersonModel
                        {
                            Id = salesPersonId.Value,
                            Name = (string)dict["SalesPersonName"]
                        };
                    }

                    myBackOrder.Invoices.Add(invoice);
                    invoiceDictionary.Add(invoice.Id, invoice);
                }
            }
        }

        return deliveryDictionary.Values.FirstOrDefault()!;
    }

    public async Task<bool> UpdateAsync(int id, DeliveryOrderCompleteRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_DeliveryOrder_Update",
            new
            {
                pId = id,
                pStatusId = request.StatusId,
                pTotal = request.Total
            },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }
}
