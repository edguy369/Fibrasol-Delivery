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
        const string query = "SELECT COUNT(Id) FROM Drivers";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query);
    }

    public async Task<int> CreateAsync(RiderRequest request)
    {
        const string query = "INSERT INTO Drivers (Name) VALUES (@Name); SELECT LAST_INSERT_ID()";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query, new { request.Name });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM Drivers WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<RiderModel>> GetAllAsync()
    {
        const string query = "SELECT * FROM Drivers";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.QueryAsync<RiderModel>(query);
    }

    public async Task<bool> UpdateAsync(int id, RiderRequest request)
    {
        const string query = "UPDATE Drivers SET Name = @Name WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id, request.Name });
        return result > 0;
    }
}
