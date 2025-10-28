using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;
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
        const string query = "SELECT COUNT(Id) FROM DeliveryOrder";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query);
    }

    public async Task<int> CreateAsync(DeliveryOrderRequest request)
    {
        const string query = "INSERT INTO DeliveryOrder (StatusId, Total) VALUES (@StatusId, @Total); SELECT LAST_INSERT_ID()";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query, new { StatusId = 1, Total = request.Total });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM DeliveryOrder WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<DeliveryOrderModel>> GetAllAsync()
    {
        const string query = "SELECT a.Id, a.Created, a.Total, a.StatusId, b.Id, b.Name From DeliveryOrder a INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.QueryAsync<DeliveryOrderModel, DeliveryOrderStatusModel, DeliveryOrderModel>(query,
            (deliveryOrder, status) =>
            {
                deliveryOrder.Status = status;
                return deliveryOrder;
            },
            splitOn: "StatusId");
    }

    public async Task<IEnumerable<DeliveryOrderModel>> GetAllUnsignedAsync()
    {
        const string query = "SELECT a.Id, a.Created, a.Total, a.StatusId, b.Id, b.Name From DeliveryOrder a INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id LEFT JOIN BackOrder c ON a.Id = c.DeliveryOrderId LEFT JOIN Invoice e ON e.BackorderId = c.Id WHERE e.SignedAttatchment = ''";
        var deliveryDictionary = new Dictionary<int, DeliveryOrderModel>();
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryAsync<DeliveryOrderModel, DeliveryOrderStatusModel, DeliveryOrderModel>(query,
            (deliveryOrder, status) =>
            {
                if (!deliveryDictionary.TryGetValue(deliveryOrder.Id, out DeliveryOrderModel? order))
                {
                    order = deliveryOrder;
                    order.Status = status;
                    deliveryDictionary.Add(order.Id, order);
                }
                return order;
            },
            splitOn: "StatusId");
        return result.Distinct();
    }

    public async Task<DeliveryOrderModel> GetByIdAsync(int id)
    {
        const string query = "SELECT a.Id, a.Created, a.Total, a.StatusId, b.Id, b.Name, f.Id AS RiderAssignationId, g.Id, g.Name, c.Id AS BackorderId, c.Id, c.Number, c.Weight, d.Id AS ClientId, d.Id, d.Name, e.Id AS InvoiceId, e.Id, e.Address, e.Reference, e.Value, e.Attatchment, e.SignedAttatchment, h.Id AS SalesPersonId, h.Id, h.Name From DeliveryOrder a INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id LEFT JOIN BackOrder c ON a.Id = c.DeliveryOrderId LEFT JOIN Clients d ON d.Id = c.ClientId LEFT JOIN Invoice e ON e.BackorderId = c.Id LEFT JOIN DeliveryOrderDrivers f ON f.DeliveryOrderId = a.Id LEFT JOIN Drivers g ON g.Id = f.DriverId LEFT JOIN SalesPerson h ON h.Id = e.SalesPersonId WHERE a.Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var deliveryDictionary = new Dictionary<int, DeliveryOrderModel>();
        var riderDictionary = new Dictionary<int, RiderModel>();
        var backOrderDictionary = new Dictionary<int, BackOrderModel>();
        var invoiceDictionary = new Dictionary<int, InvoiceModel>();

        var result = await conn.QueryAsync<DeliveryOrderModel, DeliveryOrderStatusModel, RiderModel, BackOrderModel, ClientModel, InvoiceModel, SalesPersonModel, DeliveryOrderModel>(query,
            (deliveryOrder, status, rider, backOrder, client, invoice, salesPerson) =>
            {
                if (!deliveryDictionary.TryGetValue(deliveryOrder.Id, out DeliveryOrderModel? order))
                {
                    order = deliveryOrder;
                    order.Status = status;
                    order.Backorders = new List<BackOrderModel>();
                    order.Riders = new List<RiderModel>();
                    deliveryDictionary.Add(order.Id, order);
                }

                if (rider != null && !riderDictionary.TryGetValue(rider.Id, out _))
                {
                    order.Riders.Add(rider);
                    riderDictionary.Add(rider.Id, rider);
                }

                if (backOrder != null)
                {
                    if (!backOrderDictionary.TryGetValue(backOrder.Id, out BackOrderModel? bo))
                    {
                        bo = backOrder;
                        bo.Invoices = new List<InvoiceModel>();
                        bo.Client = client;
                        order.Backorders.Add(bo);
                        backOrderDictionary.Add(bo.Id, bo);
                    }

                    if (invoice != null && !invoiceDictionary.TryGetValue(invoice.Id, out _))
                    {
                        invoice.SalesPerson = salesPerson;
                        bo.Invoices.Add(invoice);
                        invoiceDictionary.Add(invoice.Id, invoice);
                    }
                }

                return order;
            },
            new { Id = id },
            splitOn: "StatusId,RiderAssignationId,BackorderId,ClientId,InvoiceId,SalesPersonId");

        return result.Distinct().FirstOrDefault();
    }

    public async Task<bool> UpdateAsync(int id, DeliveryOrderCompleteRequest request)
    {
        const string query = "UPDATE DeliveryOrder SET StatusId = @StatusId, Total = @Total WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id, request.StatusId, request.Total });
        return result > 0;
    }
}
