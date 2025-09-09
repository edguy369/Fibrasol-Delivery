using MySql.Data.MySqlClient;
using System.Data;

namespace Fibrasol_Delivery.AuthProvider.Includes
{
    public class SqlServerDbConnectionFactory : IDbConnectionFactory
    {
        public string ConnectionString { get; set; } = default!;

        public IDbConnection Create()
        {
            var sqlConnection = new MySqlConnection(ConnectionString);
            sqlConnection.Open();
            return sqlConnection;
        }
    }
}
