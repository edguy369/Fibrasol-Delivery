using Dapper;
using Fibrasol_Delivery.Config;
using Fibrasol_Delivery.Repository.Abstract;
using MySql.Data.MySqlClient;

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
        const string query = "INSERT INTO DeliveryOrderDrivers (DeliveryOrderId, DriverId) VALUES (@pDeliveryOrderId, @pDriverId);";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pDeliveryOrderId = deliveryOrderId,
            pDriverId = driverId
        });
        return transactionResult != 0;
    }

    public async Task<bool> DeassignAsync(int driverId, int deliveryOrderId)
    {
        const string query = "DELETE FROM DeliveryOrderDrivers WHERE DeliveryOrderId = @pDeliveryOrderId AND DriverId = @pDriverId;";
        using var conn = new MySqlConnection(_connectionString);
        var transactionResult = await conn.ExecuteAsync(query, new
        {
            pDeliveryOrderId = deliveryOrderId,
            pDriverId = driverId
        });
        return transactionResult != 0;
    }
}
