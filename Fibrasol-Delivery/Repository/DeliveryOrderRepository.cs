using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Fibrasol_Delivery.Repository;

public class DeliveryOrderRepository : IDeliveryOrderRepository
{
    private readonly string _connectionString;
    public DeliveryOrderRepository(ConnectionString connectionString)
    {
        _connectionString = connectionString.Value;
    }
    public async Task<int> CreateAsync(DeliveryOrderRequest request)
    {
        const string query = "INSERT INTO DeliveryOrder (StatusId, Total) " +
            "VALUES (@pStatusId, @pTotal); SELECT LAST_INSERT_ID();";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query, new
        {
            pStatusId = 1,
            pTotal = request.Total
        });
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM DeliveryOrder WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id
        });
        return transactionResult != 0;
    }

    public async Task<IEnumerable<DeliveryOrderModel>> GetAllAsync()
    {
        const string query = "SELECT a.Id, a.Created, a.Total, a.StatusId, b.Id, b.Name From DeliveryOrder a INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<DeliveryOrderModel, DeliveryOrderStatusModel, DeliveryOrderModel>(query,
        (deliveryOrder, status) =>
        {
            deliveryOrder.Status = status;
            return deliveryOrder;
        },
        splitOn:  "StatusId");
        return transactionResult;
    }

    public async Task<DeliveryOrderModel> GetByIdAsync(int id)
    {
        const string query = "SELECT a.Id, a.Created, a.Total, a.StatusId, b.Id, b.Name, f.Id AS RiderAssignationId, g.Id, g.Name, c.Id AS BackorderId, c.Id, c.Number, c.Weight, d.Id AS ClientId, d.Id, d.Name, e.Id AS InvoiceId, e.Id, e.Address, e.Reference, e.Value, e.Attatchment, e.SignedAttatchment From DeliveryOrder a INNER JOIN DeliveryOrderStatus b ON a.StatusId = b.Id LEFT JOIN BackOrder c ON a.Id = c.DeliveryOrderId LEFT JOIN Clients d ON d.Id = c.ClientId LEFT JOIN Invoice e ON e.BackorderId = c.Id LEFT JOIN DeliveryOrderDrivers f ON f.DeliveryOrderId = a.Id LEFT JOIN Drivers g ON g.Id = f.DriverId WHERE a.Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var deliveryDisctionary = new Dictionary<int, DeliveryOrderModel>();
        var riderDisctionary = new Dictionary<int, RiderModel>();
        var backOrderDictionary = new Dictionary<int, BackOrderModel>();
        var invoiceDictionary = new Dictionary<int, InvoiceModel>();
        var transactionResult = await conn.QueryAsync<DeliveryOrderModel, DeliveryOrderStatusModel, RiderModel, BackOrderModel, ClientModel, InvoiceModel, DeliveryOrderModel >(query,
        (deliveryOrder, status, rider, backOrder, client, invoice) =>
        {
            if (!deliveryDisctionary.TryGetValue(deliveryOrder.Id, out DeliveryOrderModel? myOrder))
            {
                myOrder = deliveryOrder;
                myOrder.Status = status;
                myOrder.Backorders = new List<BackOrderModel>();
                myOrder.Riders = new List<RiderModel>();
                deliveryDisctionary.Add(myOrder.Id, myOrder);
            }

            if(rider != null)
            {
                if (!riderDisctionary.TryGetValue(rider.Id, out RiderModel? myRider))
                {
                    myRider = rider;
                    myOrder.Riders.Add(myRider);
                    riderDisctionary.Add(myRider.Id, myRider);
                }
            }

            if (backOrder != null)
            {
                if (!backOrderDictionary.TryGetValue(backOrder.Id, out BackOrderModel? myBackOrder))
                {
                    myBackOrder = backOrder;
                    myBackOrder.Invoices = new List<InvoiceModel>();
                    myBackOrder.Client = client;
                    myOrder.Backorders.Add(myBackOrder);
                    backOrderDictionary.Add(myBackOrder.Id, myBackOrder);
                }

                if (invoice != null)
                {
                    if (!invoiceDictionary.TryGetValue(invoice.Id, out InvoiceModel? myInvoice))
                    {
                        myInvoice = invoice;
                        myBackOrder.Invoices.Add(myInvoice);
                        invoiceDictionary.Add(myInvoice.Id, myInvoice);
                    }
                }
            }

            return myOrder;
        },
        new
        {
            pId = id
        },
        splitOn: "StatusId,RiderAssignationId,BackorderId,ClientId,InvoiceId");
        return transactionResult.Distinct().FirstOrDefault();
    }

    public async Task<bool> UpdateAsync(int id, DeliveryOrderCompleteRequest request)
    {
        const string query = "UPDATE DeliveryOrder SET StatusId = @pStatusId, Total = @pTotal " +
            "WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id,
            pStatusId = request.StatusId,
            pTotal = request.Total
        });
        return transactionResult != 0;
    }
}
