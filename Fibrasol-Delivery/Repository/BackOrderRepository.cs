using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;
using System.Data;

namespace Fibrasol_Delivery.Repository;

public class BackOrderRepository : IBackOrderRepository
{
    private readonly string _connectionString;

    public BackOrderRepository(ConnectionString connectionString)
    {
        _connectionString = connectionString.Value;
    }

    public async Task<int> CreateAsync(BackOrderRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_BackOrder_Create",
            new
            {
                pClientId = request.ClientId ?? 0,
                pDeliveryOrderId = request.OrderId,
                pNumber = request.Number,
                pWeight = request.Weight
            },
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_BackOrder_Delete",
            new { pId = id },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }

    public async Task<bool> UpdateAsync(int id, BackOrderRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_BackOrder_Update",
            new
            {
                pId = id,
                pClientId = request.ClientId ?? 0,
                pNumber = request.Number,
                pWeight = request.Weight
            },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }
}
