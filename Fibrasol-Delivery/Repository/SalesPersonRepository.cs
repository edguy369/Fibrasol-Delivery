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
        const string query = "SELECT COUNT(Id) FROM SalesPerson";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query);
    }

    public async Task<int> CreateAsync(SalesPersonRequest request)
    {
        const string query = "INSERT INTO SalesPerson (Name) VALUES (@Name); SELECT LAST_INSERT_ID()";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query, new { request.Name });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM SalesPerson WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<SalesPersonModel>> GetAllAsync()
    {
        const string query = "SELECT * FROM SalesPerson";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.QueryAsync<SalesPersonModel>(query);
    }

    public async Task<SalesPersonModel> GetByName(string name)
    {
        const string query = "SELECT * FROM SalesPerson WHERE Name = @Name";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<SalesPersonModel>(query, new { Name = name });
    }

    public async Task<bool> UpdateAsync(int id, SalesPersonRequest request)
    {
        const string query = "UPDATE SalesPerson SET Name = @Name WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id, request.Name });
        return result > 0;
    }

    public async Task<IEnumerable<SalesReportModel>> GetSalesReportAsync(DateTime startDate, DateTime endDate)
    {
        const string query = @"
            SELECT
                sp.Id,
                sp.Name,
                COALESCE(SUM(CASE
                    WHEN do.Created >= @StartDate AND do.Created <= @EndDate
                    THEN i.Value
                    ELSE 0
                END), 0) as TotalSales
            FROM SalesPerson sp
            LEFT JOIN Invoice i ON sp.Id = i.SalesPersonId
            LEFT JOIN BackOrder bo ON i.BackorderId = bo.Id
            LEFT JOIN DeliveryOrder do ON bo.DeliveryOrderId = do.Id
            GROUP BY sp.Id, sp.Name
            ORDER BY TotalSales DESC";

        using var conn = new MySqlConnection(_connectionString);

        var result = await conn.QueryAsync<dynamic>(query, new
        {
            StartDate = startDate.Date,
            EndDate = endDate.Date.AddDays(1).AddTicks(-1)
        });

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