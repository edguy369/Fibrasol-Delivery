using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;

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
        const string query = "INSERT INTO DeliveryOrderStatus (Name) VALUES (@Name); SELECT LAST_INSERT_ID()";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query, new { request.Name });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM DeliveryOrderStatus WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<DeliveryOrderStatusModel>> GetAllAsync()
    {
        const string query = "SELECT * FROM DeliveryOrderStatus";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.QueryAsync<DeliveryOrderStatusModel>(query);
    }

    public async Task<bool> UpdateAsync(int id, DeliveryOrderStatusRequest request)
    {
        const string query = "UPDATE DeliveryOrderStatus SET Name = @Name WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id, request.Name });
        return result > 0;
    }
}
