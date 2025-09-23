using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;

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
        const string query = "INSERT INTO BackOrder (ClientId, DeliveryOrderId, Number, Weight) " +
            "VALUES (@pClientId, @pDeliveryOrderId, @pNumber, @pWeight); SELECT LAST_INSERT_ID();";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query, new
        {
            pClientId = request.ClientId,
            pDeliveryOrderId = request.OrderId,
            pNumber = request.Number,
            pWeight = request.Weight
        });
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM BackOrder WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id
        });
        return transactionResult != 0;
    }

    public async Task<bool> UpdateAsync(int id, BackOrderRequest request)
    {
        const string query = "UPDATE BackOrder SET ClientId = @pClientId, Number = @pNumber, Weight = @pWeight " +
            "WHERE Id = @pId";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id,
            pNumber = request.Number,
            pClientId = request.ClientId,
            pWeight = request.Weight
        });
        return transactionResult != 0;
    }
}
