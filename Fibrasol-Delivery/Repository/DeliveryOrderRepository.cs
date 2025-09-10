using Dapper;
using Fibrasol_Delivery.Config;
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

    public async Task<bool> UpdateAsync(int id, DeliveryOrderRequest request)
    {
        const string query = "UPDATE DeliveryOrder SET StatusId = @pStatusId, Total = @pTotal " +
            "WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id,
            pStatusId = 1,
            pTotal = request.Total
        });
        return transactionResult != 0;
    }
}
