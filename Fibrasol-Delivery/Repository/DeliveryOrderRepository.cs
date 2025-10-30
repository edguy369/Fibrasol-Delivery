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
        var transactionResult = await conn.QueryAsync<DeliveryOrderModel, DeliveryOrderStatusModel, RiderModel, BackOrderModel, ClientModel, InvoiceModel, SalesPersonModel, DeliveryOrderModel>(
        "sp_DeliveryOrder_GetById",
        (deliveryOrder, status, rider, backOrder, client, invoice, salesPerson) =>
        {
            if (!deliveryDisctionary.TryGetValue(deliveryOrder.Id, out DeliveryOrderModel? myOrder))
            {
                myOrder = deliveryOrder;
                myOrder.Status = status;
                myOrder.Backorders = new List<BackOrderModel>();
                myOrder.Riders = new List<RiderModel>();
                deliveryDisctionary.Add(myOrder.Id, myOrder);
            }

                if (rider != null && !riderDictionary.TryGetValue(rider.Id, out _))
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

                    if (invoice != null && !invoiceDictionary.TryGetValue(invoice.Id, out _))
                    {
                        invoice.SalesPerson = salesPerson;
                        bo.Invoices.Add(invoice);
                        invoiceDictionary.Add(invoice.Id, invoice);
                    }
                }
            }

            return myOrder;
        },
        new
        {
            pId = id
        },
        commandType: CommandType.StoredProcedure,
        splitOn: "StatusId,RiderAssignationId,BackorderId,ClientId,InvoiceId,SalesPersonId");
        return transactionResult.Distinct().FirstOrDefault();
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
