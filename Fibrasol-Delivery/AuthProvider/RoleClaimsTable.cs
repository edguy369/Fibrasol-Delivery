using Fibrasol_Delivery.AuthProvider.Abstract;
using Fibrasol_Delivery.Config;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;

namespace Fibrasol_Delivery.AuthProvider;

public class RoleClaimsTable<TKey, TRoleClaim> :
   IdentityTable,
   IRoleClaimsTable<TKey, TRoleClaim>
   where TKey : IEquatable<TKey>
   where TRoleClaim : IdentityRoleClaim<TKey>, new()
{
    private readonly ConnectionString _connectionString;
    public RoleClaimsTable(ConnectionString connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }
    public virtual async Task<IEnumerable<TRoleClaim>> GetClaimsAsync(TKey roleId)
    {
        const string sql = "SELECT * " +
                           "FROM RoleClaims " +
                           "WHERE RoleId = @RoleId;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var roleClaims = await conn.QueryAsync<TRoleClaim>(sql, new { RoleId = roleId });
            return roleClaims;
        }

    }
}
