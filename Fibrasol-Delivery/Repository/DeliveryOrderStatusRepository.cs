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
        const string query = "INSERT INTO DeliveryOrderStatus (Name) VALUES (@pName); SELECT LAST_INSERT_ID();";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query, new
        {
            pName = request.Name
        });
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM DeliveryOrderStatus WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id
        });
        return transactionResult != 0;
    }

    public async Task<IEnumerable<DeliveryOrderStatusModel>> GetAllAsync()
    {
        const string query = "SELECT * FROM DeliveryOrderStatus;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<DeliveryOrderStatusModel>(query);
        return transactionResult;
    }

    public async Task<bool> UpdateAsync(int id, DeliveryOrderStatusRequest request)
    {
        const string query = "UPDATE DeliveryOrderStatus SET Name = @pName WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id,
            pName = request.Name
        });
        return transactionResult != 0;
    }
}
