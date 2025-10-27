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
        const string query = "SELECT COUNT(Id) FROM Clients;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query);
        return transactionResult;
    }

    public async Task<int> CreateAsync(ClientRequest request)
    {
        const string query = "INSERT INTO Clients (Name) VALUES (@pName); SELECT LAST_INSERT_ID();";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query, new
        {
            pName = request.Name
        });
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM Clients WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id
        });
        return transactionResult != 0;
    }

    public async Task<IEnumerable<SalesPersonModel>> GetAllAsync()
    {
        const string query = "SELECT * FROM Clients;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<SalesPersonModel>(query);
        return transactionResult;
    }

    public async Task<SalesPersonModel> GetByName(string name)
    {
        const string query = "SELECT * FROM Clients WHERE Name = @pName;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryFirstOrDefaultAsync<SalesPersonModel>(query,
        new {
            pName = name
        });
        return transactionResult!;
    }

    public async Task<bool> UpdateAsync(int id, ClientRequest request)
    {
        const string query = "UPDATE Clients SET Name = @pName WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id,
            pName = request.Name
        });
        return transactionResult != 0;
    }
}
