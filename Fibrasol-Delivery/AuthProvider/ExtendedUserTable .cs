using Dapper;
using Fibrasol_Delivery.AuthProvider.Extensions;
using Fibrasol_Delivery.Config;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System.Data;
namespace Fibrasol_Delivery.AuthProvider;

public class ExtendedUserTable : UsersTable<ExtendedIdentityUser, string, IdentityUserClaim<string>,
IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>>
{
    private readonly string _connectionString;
    public ExtendedUserTable(ConnectionString connectionString) : base(connectionString)
    {
        _connectionString = connectionString.Value;
    }

    public override async Task<bool> CreateAsync(ExtendedIdentityUser user)
    {
        const string sql = "INSERT INTO Users " +
                           "VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, " +
                                   "@PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, @AccessFailedCount, @Name, @Surname, @IsContaEnabled);";
        using (var conn = new MySqlConnection(_connectionString))
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
                user.AccessFailedCount,
                user.Name,
                user.Surname,
                user.IsContaEnabled
            });
            return rowsInserted == 1;
        }

    }

    /// <inheritdoc/>
    public override Task<bool> UpdateAsync(ExtendedIdentityUser user, IList<IdentityUserClaim<string>> claims, IList<IdentityUserLogin<string>> logins, IList<IdentityUserToken<string>> tokens)
    {
        return UpdateAsync(user, claims, null, logins, tokens);
    }

    /// <inheritdoc/>
    public override async Task<bool> UpdateAsync(ExtendedIdentityUser user, IList<IdentityUserClaim<string>> claims, IList<IdentityUserRole<string>> roles, IList<IdentityUserLogin<string>> logins, IList<IdentityUserToken<string>> tokens)
    {
        const string updateUserSql =
            "UPDATE Users " +
            "SET UserName = @UserName, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail, EmailConfirmed = @EmailConfirmed, " +
                "PasswordHash = @PasswordHash, SecurityStamp = @SecurityStamp, ConcurrencyStamp = @ConcurrencyStamp, PhoneNumber = @PhoneNumber, " +
                "PhoneNumberConfirmed = @PhoneNumberConfirmed, TwoFactorEnabled = @TwoFactorEnabled, LockoutEnd = @LockoutEnd, LockoutEnabled = @LockoutEnabled, " +
                "AccessFailedCount = @AccessFailedCount, Name = @Name, Surname = @Surname, IsContaEnabled = @IsContaEnabled " +
            "WHERE Id = @Id;";
        using (var conn = new MySqlConnection(_connectionString))
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
                    user.Name,
                    user.Surname,
                    user.IsContaEnabled,
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
}
