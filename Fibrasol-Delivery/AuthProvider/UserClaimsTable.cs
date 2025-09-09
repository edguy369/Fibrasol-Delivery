using Fibrasol_Delivery.AuthProvider.Abstract;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using Fibrasol_Delivery.Config;

namespace Fibrasol_Delivery.AuthProvider;

public class UserClaimsTable<TKey, TUserClaim> :
    IdentityTable,
    IUserClaimsTable<TKey, TUserClaim>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>, new()
{
    private readonly ConnectionString _connectionString;
    public UserClaimsTable(ConnectionString connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<TUserClaim>> GetClaimsAsync(TKey userId)
    {
        const string sql = "SELECT * " +
                           "FROM UserClaims " +
                           "WHERE UserId = @UserId;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var userClaims = await conn.QueryAsync<TUserClaim>(sql, new { UserId = userId });
            return userClaims;
        }
    }
}
