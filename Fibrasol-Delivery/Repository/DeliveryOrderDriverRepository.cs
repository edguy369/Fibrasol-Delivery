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
        const string query = "INSERT INTO DeliveryOrderDrivers (DeliveryOrderId, DriverId) VALUES (@DeliveryOrderId, @DriverId)";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { DeliveryOrderId = deliveryOrderId, DriverId = driverId });
        return result > 0;
    }

    public async Task<bool> DeassignAsync(int driverId, int deliveryOrderId)
    {
        const string query = "DELETE FROM DeliveryOrderDrivers WHERE DeliveryOrderId = @DeliveryOrderId AND DriverId = @DriverId";
        using var conn = new MySqlConnection(_connectionString);
        var result = await conn.ExecuteAsync(query, new { DeliveryOrderId = deliveryOrderId, DriverId = driverId });
        return result > 0;
    }
}
