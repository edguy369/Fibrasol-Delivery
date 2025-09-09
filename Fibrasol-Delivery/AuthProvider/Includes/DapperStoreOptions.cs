using Microsoft.Extensions.DependencyInjection;

namespace Fibrasol_Delivery.AuthProvider.Includes;

public class DapperStoreOptions
{
    internal IServiceCollection Services = default!;
    /// <summary>
    /// The connection string to use for connecting to the data source.
    /// </summary>
    public string ConnectionString { get; set; } = default!;
    /// <summary>
    /// A factory for creating instances of <see cref="IDbConnection"/>.
    /// </summary>
    public IDbConnectionFactory DbConnectionFactory { get; set; } = default!;
}
