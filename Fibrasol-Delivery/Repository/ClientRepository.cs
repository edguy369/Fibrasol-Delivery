using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;

namespace Fibrasol_Delivery.Repository;

public class ClientRepository : IClientRepository
{
    private readonly string _connectionString;

    public ClientRepository(ConnectionString connectionString)
    {
        _connectionString = connectionString.Value;
    }

    public async Task<int> CountAsync()
    {
        const string query = "SELECT COUNT(Id) FROM Clients";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query);
    }

    public async Task<int> CreateAsync(ClientRequest request)
    {
        const string query = "INSERT INTO Clients (Name) VALUES (@Name); SELECT LAST_INSERT_ID()";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query, new { request.Name });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM Clients WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<SalesPersonModel>> GetAllAsync()
    {
        const string query = "SELECT * FROM Clients";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.QueryAsync<SalesPersonModel>(query);
    }

    public async Task<SalesPersonModel> GetByName(string name)
    {
        const string query = "SELECT * FROM Clients WHERE Name = @Name";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<SalesPersonModel>(query, new { Name = name });
    }

    public async Task<bool> UpdateAsync(int id, ClientRequest request)
    {
        const string query = "UPDATE Clients SET Name = @Name WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id, request.Name });
        return result > 0;
    }
}
