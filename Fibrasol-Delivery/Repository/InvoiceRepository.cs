using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Repository.Abstract;
using Fibrasol_Delivery.Request;
using MySql.Data.MySqlClient;
using System.Data;

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
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_Invoice_Count",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<int> CountSignedAsync()
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_Invoice_CountSigned",
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<int> CreateAsync(InvoiceRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteScalarAsync<int>(
            "sp_Invoice_Create",
            new
            {
                pBackorderId = request.BackorderId,
                pAddress = request.Address,
                pReference = request.Reference,
                pValue = request.Value,
                pAttatchment = request.Attatchment,
                pSignedAttatchment = request.SignedAttatchment,
                pSalesPersonId = request.SalesPersonId
            },
            commandType: CommandType.StoredProcedure);
        return transactionResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_Invoice_Delete",
            new { pId = id },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }

    public async Task<bool> UpdateAsync(int id, InvoiceRequest request)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_Invoice_Update",
            new
            {
                pId = id,
                pAddress = request.Address,
                pReference = request.Reference,
                pValue = request.Value,
                pAttatchment = request.Attatchment,
                pSignedAttatchment = request.SignedAttatchment,
                pSalesPersonId = request.SalesPersonId
            },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }
}
