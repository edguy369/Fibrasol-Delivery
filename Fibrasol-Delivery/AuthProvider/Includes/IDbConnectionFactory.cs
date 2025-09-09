using System.Data;

namespace Fibrasol_Delivery.AuthProvider.Includes
{
    public interface IDbConnectionFactory
    {
        public string ConnectionString { get; set; } 
        IDbConnection Create();
    }
}
