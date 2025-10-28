using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;

namespace Fibrasol_Delivery.Repository;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly string _connectionString;

    public InvoiceRepository(ConnectionString connectionString)
    {
        _connectionString = connectionString.Value;
    }

    public async Task<int> CountAsync()
    {
        const string query = "SELECT COUNT(Id) FROM Invoice";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query);
    }

    public async Task<int> CountSignedAsync()
    {
        const string query = "SELECT COUNT(Id) FROM Invoice WHERE SignedAttatchment != ''";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query);
    }

    public async Task<int> CreateAsync(InvoiceRequest request)
    {
        const string query = "INSERT INTO Invoice (BackorderId, Address, Reference, Value, Attatchment, SignedAttatchment, SalesPersonId) VALUES (@BackorderId, @Address, @Reference, @Value, @Attatchment, @SignedAttatchment, @SalesPersonId); SELECT LAST_INSERT_ID()";
        using var conn = new MySqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(query, new
        {
            request.BackorderId,
            request.Address,
            request.Reference,
            request.Value,
            request.Attatchment,
            request.SignedAttatchment,
            request.SalesPersonId
        });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM Invoice WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { Id = id });
        return result > 0;
    }

    public async Task<bool> UpdateAsync(int id, InvoiceRequest request)
    {
        const string query = "UPDATE Invoice SET Address = @Address, Reference = @Reference, Value = @Value, Attatchment = @Attatchment, SignedAttatchment = @SignedAttatchment, SalesPersonId = @SalesPersonId WHERE Id = @Id";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new
        {
            Id = id,
            request.Address,
            request.Reference,
            request.Value,
            request.Attatchment,
            request.SignedAttatchment,
            request.SalesPersonId
        });
        return result > 0;
    }
}
