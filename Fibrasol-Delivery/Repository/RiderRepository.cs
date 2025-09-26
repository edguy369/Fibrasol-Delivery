using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;

namespace Fibrasol_Delivery.Repository;

public class RiderRepository : IRiderRepository
{
    private readonly string _connectionString;
    public RiderRepository(ConnectionString connectionString)
    {
        _connectionString = connectionString.Value;
    }

    public async Task<int> CountAsync()
    {
        const string query = "SELECT COUNT(Id) FROM Drivers;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query);
        return transactionResult;
    }

    public async Task<int> CreateAsync(RiderRequest request)
    {
        const string query = "INSERT INTO Drivers (Name) VALUES (@pName); SELECT LAST_INSERT_ID();";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query, new
        {
            pName = request.Name
        });
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM Drivers WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id
        });
        return transactionResult != 0;
    }

    public async Task<IEnumerable<RiderModel>> GetAllAsync()
    {
        const string query = "SELECT * FROM Drivers;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<RiderModel>(query);
        return transactionResult;
    }

    public async Task<bool> UpdateAsync(int id, RiderRequest request)
    {
        const string query = "UPDATE Drivers SET Name = @pName WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id,
            pName = request.Name
        });
        return transactionResult != 0;
    }
}
