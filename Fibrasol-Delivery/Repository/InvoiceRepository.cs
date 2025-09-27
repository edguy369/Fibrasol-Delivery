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
        const string query = "SELECT COUNT(Id) FROM Invoice;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query);
        return transactionResult;
    }

    public async Task<int> CountSignedAsync()
    {
        const string query = "SELECT COUNT(Id) FROM Invoice WHERE SignedAttatchment != '';";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(query);
        return transactionResult;
    }

    public async Task<int> CreateAsync(InvoiceRequest request)
    {
        try
        {
            const string query = "INSERT INTO Invoice (BackorderId, Address, Reference, Value, Attatchment, SignedAttatchment, SalesPersonId) " +
            "VALUES (@pBackorderId, @pAddress, @pReference, @pValue, @pAttatchment, @pSignedAttatchment, @pSalesPersonId); SELECT LAST_INSERT_ID();";
            using var conn = new MySqlConnection(_connectionString);
            var transactionResult = await conn.ExecuteScalarAsync<int>(query, new
            {
                pBackorderId = request.BackorderId,
                pAddress = request.Address,
                pReference = request.Reference,
                pValue = request.Value,
                pAttatchment = request.Attatchment,
                pSignedAttatchment = request.SignedAttatchment,
                pSalesPersonId = request.SalesPersonId
            });
            return transactionResult;
        }
        catch (Exception e)
        {
            throw e;
        }
        
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = "DELETE FROM Invoice WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id
        });
        return transactionResult != 0;
    }

    public async Task<bool> UpdateAsync(int id, InvoiceRequest request)
    {
        const string query = "UPDATE Invoice SET Address = @pAddress, Reference = @pReference, Value = @pValue, Attatchment = @pAttatchment, " +
            "SignedAttatchment = @pSignedAttatchment, SalesPersonId = @pSalesPersonId WHERE Id = @pId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pId = id,
            pAddress = request.Address,
            pReference = request.Reference,
            pValue = request.Value,
            pAttatchment = request.Attatchment,
            pSignedAttatchment = request.SignedAttatchment,
            pSalesPersonId = request.SalesPersonId
        });
        return transactionResult != 0;
    }
}
