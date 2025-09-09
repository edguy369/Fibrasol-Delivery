using Fibrasol_Delivery.Config;
using MySql.Data.MySqlClient;

namespace Fibrasol_Delivery.AuthProvider;

public class IdentityTable : TableBase
{
    private readonly MySqlConnection _connectionString;
    public IdentityTable(ConnectionString connectionString)
    {
        _connectionString = new MySqlConnection(connectionString.Value);
    }

    protected override void OnDispose()
    {
        if (_connectionString != null)
        {
            _connectionString.Dispose();
        }
    }
}
