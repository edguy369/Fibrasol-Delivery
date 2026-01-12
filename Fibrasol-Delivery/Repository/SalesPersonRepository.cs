using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Models;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;
using System.Data;

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
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_SalesPerson_Count",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<int> CreateAsync(SalesPersonRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_SalesPerson_Create",
            new { pName = request.Name },
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_SalesPerson_Delete",
            new { pId = id },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }

    public async Task<IEnumerable<SalesPersonModel>> GetAllAsync()
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryAsync<SalesPersonModel>(
            "sp_SalesPerson_GetAll",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<SalesPersonModel> GetByName(string name)
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.QueryFirstOrDefaultAsync<SalesPersonModel>(
            "sp_SalesPerson_GetByName",
            new { pName = name },
            commandType: CommandType.StoredProcedure);
        return transactionResult!;
    }

    public async Task<bool> UpdateAsync(int id, SalesPersonRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_SalesPerson_Update",
            new { pId = id, pName = request.Name },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }

    public async Task<IEnumerable<SalesReportModel>> GetSalesReportAsync(DateTime startDate, DateTime endDate)
    {
        using var conn = new MySqlConnection(_connectionString);

        var result = await conn.QueryAsync<dynamic>(
            "sp_SalesPerson_GetSalesReport",
            new
            {
                pStartDate = startDate.Date,
                pEndDate = endDate.Date.AddDays(1).AddTicks(-1)
            },
            commandType: CommandType.StoredProcedure);

        return result.Select(row => new SalesReportModel
        {
            SalesPerson = new SalesPersonModel
            {
                Id = (int)row.Id,
                Name = (string)row.Name
            },
            TotalSales = (double)(row.TotalSales ?? 0)
        });
    }
}