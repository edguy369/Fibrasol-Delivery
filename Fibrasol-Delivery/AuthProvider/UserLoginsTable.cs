using Fibrasol_Delivery.AuthProvider.Abstract;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using Fibrasol_Delivery.Config;

namespace Fibrasol_Delivery.AuthProvider;

public class UserLoginsTable<TUser, TKey, TUserLogin> :
    IdentityTable,
    IUserLoginsTable<TUser, TKey, TUserLogin>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TUserLogin : IdentityUserLogin<TKey>, new()
{
    private readonly ConnectionString _connectionString;
    public UserLoginsTable(ConnectionString connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<TUserLogin>> GetLoginsAsync(TKey userId)
    {
        const string sql = "SELECT * " +
                           "FROM UserLogins " +
                           "WHERE UserId = @UserId;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var userLogins = await conn.QueryAsync<TUserLogin>(sql, new { UserId = userId });
            return userLogins;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey)
    {
        const string sql = "SELECT u.* " +
                           "FROM Users AS [u] " +
                           "INNER JOIN UserLogins AS ul ON ul.UserId = u.Id " +
                           "WHERE ul.LoginProvider = @LoginProvider AND ul.ProviderKey = @ProviderKey;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var user = await conn.QuerySingleOrDefaultAsync<TUser>(sql, new
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return user;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey)
    {
        const string sql = "SELECT * " +
                           "FROM UserLogins " +
                           "WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var userLogin = await conn.QuerySingleOrDefaultAsync<TUserLogin>(sql, new
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return userLogin;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey)
    {
        const string sql = "SELECT * " +
                           "FROM UserLogins " +
                           "WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var userLogin = await conn.QuerySingleOrDefaultAsync<TUserLogin>(sql, new
            {
                UserId = userId,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return userLogin;
        }

    }
}
