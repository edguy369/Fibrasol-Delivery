using Fibrasol_Delivery.AuthProvider.Abstract;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using Fibrasol_Delivery.Config;

namespace Fibrasol_Delivery.AuthProvider;

public class UserTokensTable<TKey, TUserToken> :
    IdentityTable,
    IUserTokensTable<TKey, TUserToken>
    where TKey : IEquatable<TKey>
    where TUserToken : IdentityUserToken<TKey>, new()
{
    private readonly ConnectionString _connectionString;
    public UserTokensTable(ConnectionString connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<TUserToken>> GetTokensAsync(TKey userId)
    {
        const string sql = "SELECT * " +
                           "FROM UserTokens " +
                           "WHERE UserId = @UserId;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var userTokens = await conn.QueryAsync<TUserToken>(sql, new { UserId = userId });
            return userTokens;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<TUserToken> FindTokenAsync(TKey userId, string loginProvider, string name)
    {
        const string sql = "SELECT * " +
                           "FROM UserTokens " +
                           "WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var token = await conn.QuerySingleOrDefaultAsync<TUserToken>(sql, new
            {
                UserId = userId,
                LoginProvider = loginProvider,
                Name = name
            });
            return token;
        }

    }
}
