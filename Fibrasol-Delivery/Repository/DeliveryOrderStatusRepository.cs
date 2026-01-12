using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;
using System.Data;

namespace Fibrasol_Delivery.Repository;

public class DeliveryOrderStatusRepository : IDeliveryOrderStatusRepository
{
    private readonly string _connectionString;

    public DeliveryOrderStatusRepository(ConnectionString connectionString)
    {
        _connectionString = connectionString.Value;
    }

    public async Task<int> CreateAsync(DeliveryOrderStatusRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_DeliveryOrderStatus_Create",
            new { pName = request.Name },
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_DeliveryOrderStatus_Delete",
            new { pId = id },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }

    public async Task<IEnumerable<DeliveryOrderStatusModel>> GetAllAsync()
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<DeliveryOrderStatusModel>(
            "sp_DeliveryOrderStatus_GetAll",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<bool> UpdateAsync(int id, DeliveryOrderStatusRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_DeliveryOrderStatus_Update",
            new { pId = id, pName = request.Name },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }
}
