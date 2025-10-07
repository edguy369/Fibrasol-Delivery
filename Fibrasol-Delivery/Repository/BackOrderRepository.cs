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
        const string query = "INSERT INTO BackOrder (ClientId, DeliveryOrderId, Number, Weight) VALUES (@ClientId, @DeliveryOrderId, @Number, @Weight); SELECT LAST_INSERT_ID()";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query, new
        {
            request.ClientId,
            DeliveryOrderId = request.OrderId,
            request.Number,
            request.Weight
        });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM BackOrder WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id });
        return result > 0;
    }

    public async Task<bool> UpdateAsync(int id, BackOrderRequest request)
    {
        const string query = "UPDATE BackOrder SET ClientId = @ClientId, Number = @Number, Weight = @Weight WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new
        {
            Id = id,
            request.ClientId,
            request.Number,
            request.Weight
        });
        return result > 0;
    }
}
