using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;
using System.Data;

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
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_Rider_Count",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<int> CreateAsync(RiderRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_Rider_Create",
            new { pName = request.Name },
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_Rider_Delete",
            new { pId = id },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }

    public async Task<IEnumerable<RiderModel>> GetAllAsync()
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<RiderModel>(
            "sp_Rider_GetAll",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<bool> UpdateAsync(int id, RiderRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_Rider_Update",
            new { pId = id, pName = request.Name },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }
}
