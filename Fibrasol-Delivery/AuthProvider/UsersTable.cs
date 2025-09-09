using Fibrasol_Delivery.AuthProvider.Abstract;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Claims;
using Fibrasol_Delivery.Config;

namespace Fibrasol_Delivery.AuthProvider;

public class UsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken> :
    IdentityTable,
    IUsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>, new()
    where TUserRole : IdentityUserRole<TKey>, new()
    where TUserLogin : IdentityUserLogin<TKey>, new()
    where TUserToken : IdentityUserToken<TKey>, new()
{
    private readonly ConnectionString _connectionString;
    public UsersTable(ConnectionString connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> CreateAsync(TUser user)
    {
        const string sql = "INSERT INTO Users " +
                           "VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, " +
                                   "@PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, @AccessFailedCount);";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var rowsInserted = await conn.ExecuteAsync(sql, new
            {
                user.Id,
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.PasswordHash,
                user.SecurityStamp,
                user.ConcurrencyStamp,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.LockoutEnd,
                user.LockoutEnabled,
                user.AccessFailedCount
            });
            return rowsInserted == 1;
        }

    }

    public virtual async Task<bool> DeleteAsync(TKey userId)
    {
        const string sql = "DELETE " +
                           "FROM Users " +
                           "WHERE Id = @Id;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var rowsDeleted = await conn.ExecuteAsync(sql, new { Id = userId });
            return rowsDeleted == 1;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<TUser> FindByIdAsync(TKey userId)
    {
        const string sql = "SELECT * " +
                           "FROM Users " +
                           "WHERE Id = @Id;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var user = await conn.QuerySingleOrDefaultAsync<TUser>(sql, new { Id = userId });
            return user;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<TUser> FindByNameAsync(string normalizedUserName)
    {
        const string sql = "SELECT * " +
                           "FROM Users " +
                           "WHERE NormalizedUserName = @NormalizedUserName;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var user = await conn.QuerySingleOrDefaultAsync<TUser>(sql, new { NormalizedUserName = normalizedUserName });
            return user;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<TUser> FindByEmailAsync(string normalizedEmail)
    {
        const string command = "SELECT * " +
                               "FROM Users " +
                               "WHERE NormalizedEmail = @NormalizedEmail;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var user = await conn.QuerySingleOrDefaultAsync<TUser>(command, new { NormalizedEmail = normalizedEmail });
            return user;
        }

    }

    /// <inheritdoc/>
    public virtual Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserLogin> logins, IList<TUserToken> tokens)
    {
        return UpdateAsync(user, claims, null, logins, tokens);
    }

    /// <inheritdoc/>
    public virtual async Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserRole> roles, IList<TUserLogin> logins, IList<TUserToken> tokens)
    {
        const string updateUserSql =
            "UPDATE Users " +
            "SET UserName = @UserName, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail, EmailConfirmed = @EmailConfirmed, " +
                "PasswordHash = @PasswordHash, SecurityStamp = @SecurityStamp, ConcurrencyStamp = @ConcurrencyStamp, PhoneNumber = @PhoneNumber, " +
                "PhoneNumberConfirmed = @PhoneNumberConfirmed, TwoFactorEnabled = @TwoFactorEnabled, LockoutEnd = @LockoutEnd, LockoutEnabled = @LockoutEnabled, " +
                "AccessFailedCount = @AccessFailedCount " +
            "WHERE Id = @Id;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            using (var transaction = conn.BeginTransaction())
            {
                await conn.ExecuteAsync(updateUserSql, new
                {
                    user.UserName,
                    user.NormalizedUserName,
                    user.Email,
                    user.NormalizedEmail,
                    user.EmailConfirmed,
                    user.PasswordHash,
                    user.SecurityStamp,
                    user.ConcurrencyStamp,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    user.TwoFactorEnabled,
                    user.LockoutEnd,
                    user.LockoutEnabled,
                    user.AccessFailedCount,
                    user.Id
                }, transaction);
                if (claims?.Count() > 0)
                {
                    const string deleteClaimsSql = "DELETE " +
                                                   "FROM UserClaims " +
                                                   "WHERE UserId = @UserId;";
                    await conn.ExecuteAsync(deleteClaimsSql, new { UserId = user.Id }, transaction);
                    const string insertClaimsSql = "INSERT INTO UserClaims (UserId, ClaimType, ClaimValue) " +
                                                   "VALUES (@UserId, @ClaimType, @ClaimValue);";
                    await conn.ExecuteAsync(insertClaimsSql, claims.Select(x => new
                    {
                        UserId = user.Id,
                        x.ClaimType,
                        x.ClaimValue
                    }), transaction);
                }
                if (roles?.Count() > 0)
                {
                    const string deleteRolesSql = "DELETE " +
                                                  "FROM UserRoles " +
                                                  "WHERE UserId = @UserId;";
                    await conn.ExecuteAsync(deleteRolesSql, new { UserId = user.Id }, transaction);
                    const string insertRolesSql = "INSERT INTO UserRoles (UserId, RoleId) " +
                                                  "VALUES (@UserId, @RoleId);";
                    await conn.ExecuteAsync(insertRolesSql, roles.Select(x => new
                    {
                        UserId = user.Id,
                        x.RoleId
                    }), transaction);
                }
                if (logins?.Count() > 0)
                {
                    const string deleteLoginsSql = "DELETE " +
                                                   "FROM UserLogins " +
                                                   "WHERE UserId = @UserId;";
                    await conn.ExecuteAsync(deleteLoginsSql, new { UserId = user.Id }, transaction);
                    const string insertLoginsSql = "INSERT INTO UserLogins (LoginProvider, ProviderKey, ProviderDisplayName, UserId) " +
                                                   "VALUES (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId);";
                    await conn.ExecuteAsync(insertLoginsSql, logins.Select(x => new
                    {
                        x.LoginProvider,
                        x.ProviderKey,
                        x.ProviderDisplayName,
                        UserId = user.Id
                    }), transaction);
                }
                if (tokens?.Count() > 0)
                {
                    const string deleteTokensSql = "DELETE " +
                                                   "FROM UserTokens " +
                                                   "WHERE UserId = @UserId;";
                    await conn.ExecuteAsync(deleteTokensSql, new { UserId = user.Id }, transaction);
                    const string insertTokensSql = "INSERT INTO UserTokens (UserId, LoginProvider, Name, Value) " +
                                                   "VALUES (@UserId, @LoginProvider, @Name, @Value);";
                    await conn.ExecuteAsync(insertTokensSql, tokens.Select(x => new
                    {
                        x.UserId,
                        x.LoginProvider,
                        x.Name,
                        x.Value
                    }), transaction);
                }
                try
                {
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<TUser>> GetUsersInRoleAsync(string roleName)
    {
        const string sql = "SELECT * " +
                           "FROM Users AS u " +
                           "INNER JOIN UserRoles AS ur ON u.Id = ur.UserId " +
                           "INNER JOIN Roles AS r ON ur.RoleId = r.Id " +
                           "WHERE r.Name = @RoleName;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var users = await conn.QueryAsync<TUser>(sql, new { RoleName = roleName });
            return users;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<TUser>> GetUsersForClaimAsync(Claim claim)
    {
        const string sql = "SELECT * " +
                               "FROM Users AS u " +
                               "INNER JOIN UserClaims AS uc ON u.Id = uc.UserId " +
                               "WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var users = await conn.QueryAsync<TUser>(sql, new
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });
            return users;
        }

    }
}
