using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Repository.Abstract;
using MySql.Data.MySqlClient;
using System.Data;

namespace Fibrasol_Delivery.Repository;

public class DeliveryOrderDriverRepository : IDeliveryOrderDriverRepository
{
    private readonly string _connectionString;
    public DeliveryOrderDriverRepository(ConnectionString connectionString)
    {
        _connectionString = connectionString.Value;
    }

    public async Task<bool> AssignAsync(int driverId, int deliveryOrderId)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_DeliveryOrderDriver_Create",
            new
            {
                pDeliveryOrderId = deliveryOrderId,
                pDriverId = driverId
            },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }

    public async Task<bool> DeassignAsync(int driverId, int deliveryOrderId)
    {
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<int>(
            "sp_DeliveryOrderDriver_Delete",
            new
            {
                pDeliveryOrderId = deliveryOrderId,
                pDriverId = driverId
            },
            commandType: CommandType.StoredProcedure);
        return result != 0;
    }
}
