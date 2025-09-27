using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;

namespace Fibrasol_Delivery.Repository;

public class SalesPersonRepository : ISalesPersonRepository
{
    private readonly string _connectionString;
    public SalesPersonRepository(ConnectionString connectionString)
    {
        _connectionString = connectionString.Value;
    }

    public async Task<int> CountAsync()
    {
        const string query = "SELECT COUNT(Id) FROM SalesPerson;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query);
        return transactionResult;
    }

    public async Task<int> CreateAsync(SalesPersonRequest request)
    {
        const string query = "INSERT INTO SalesPerson (Name) VALUES (@pName); SELECT LAST_INSERT_ID();";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query, new
        {
            pName = request.Name
        });
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM SalesPerson WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id
        });
        return transactionResult != 0;
    }

    public async Task<IEnumerable<SalesPersonModel>> GetAllAsync()
    {
        const string query = "SELECT * FROM SalesPerson;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<SalesPersonModel>(query);
        return transactionResult;
    }

    public async Task<SalesPersonModel> GetByName(string name)
    {
        const string query = "SELECT * FROM SalesPerson WHERE Name = @pName;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryFirstOrDefaultAsync<SalesPersonModel>(query,
        new {
            pName = name
        });
        return transactionResult;
    }

    public async Task<bool> UpdateAsync(int id, SalesPersonRequest request)
    {
        const string query = "UPDATE SalesPerson SET Name = @pName WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id,
            pName = request.Name
        });
        return transactionResult != 0;
    }
}