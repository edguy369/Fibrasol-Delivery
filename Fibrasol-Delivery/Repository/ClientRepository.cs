using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;
using System.Data;

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
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_Client_Count",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<int> CreateAsync(ClientRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_Client_Create",
            new { pName = request.Name },
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_Client_Delete",
            new { pId = id },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }

    public async Task<IEnumerable<SalesPersonModel>> GetAllAsync()
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<SalesPersonModel>(
            "sp_Client_GetAll",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<SalesPersonModel> GetByName(string name)
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryFirstOrDefaultAsync<SalesPersonModel>(
            "sp_Client_GetByName",
            new { pName = name },
            commandType: CommandType.StoredProcedure);
        return transactionResult!;
    }

    public async Task<bool> UpdateAsync(int id, ClientRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_Client_Update",
            new { pId = id, pName = request.Name },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }
}
