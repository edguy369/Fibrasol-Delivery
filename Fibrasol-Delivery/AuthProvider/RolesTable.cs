using Fibrasol_Delivery.AuthProvider.Abstract;
using Fibrasol_Delivery.Config;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;

namespace Fibrasol_Delivery.AuthProvider;

public class RolesTable<TRole, TKey, TRoleClaim> :
    IdentityTable,
    IRolesTable<TRole, TKey, TRoleClaim>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>, new()
{
    private readonly ConnectionString _connectionString;
    public RolesTable(ConnectionString connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }

    public virtual async Task<bool> CreateAsync(TRole role)
    {
        const string sql = "INSERT INTO Roles (Id, Name, NormalizedName, ConcurrencyStamp) " +
                           "VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp);";

        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var rowsInserted = await conn.ExecuteAsync(sql, new
            {
                role.Id,
                role.Name,
                role.NormalizedName,
                role.ConcurrencyStamp
            });
            return rowsInserted == 1;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<bool> DeleteAsync(TKey roleId)
    {
        const string sql = "DELETE " +
                           "FROM Roles " +
                           "WHERE Id = @Id;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var rowsDeleted = await conn.ExecuteAsync(sql, new { Id = roleId });
            return rowsDeleted == 1;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<TRole> FindByIdAsync(TKey roleId)
    {
        const string sql = "SELECT * " +
                           "FROM Roles " +
                           "WHERE Id = @Id;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var role = await conn.QuerySingleOrDefaultAsync<TRole>(sql, new { Id = roleId });
            return role;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<TRole> FindByNameAsync(string normalizedName)
    {
        const string sql = "SELECT * " +
                           "FROM Roles " +
                           "WHERE NormalizedName = @NormalizedName;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var role = await conn.QuerySingleOrDefaultAsync<TRole>(sql, new { NormalizedName = normalizedName });
            return role;
        }

    }

    /// <inheritdoc/>
    public virtual async Task<bool> UpdateAsync(TRole role, IList<TRoleClaim>? claims = null)
    {
        const string updateRoleSql = "UPDATE Roles " +
                                     "SET Name = @Name, NormalizedName = @NormalizedName, ConcurrencyStamp = @ConcurrencyStamp " +
                                     "WHERE Id = @Id;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            using (var transaction = conn.BeginTransaction())
            {
                await conn.ExecuteAsync(updateRoleSql, new
                {
                    role.Name,
                    role.NormalizedName,
                    role.ConcurrencyStamp,
                    role.Id
                }, transaction);
                if (claims?.Count() > 0)
                {
                    const string deleteClaimsSql = "DELETE " +
                                                   "FROM RoleClaims " +
                                                   "WHERE RoleId = @RoleId;";
                    await conn.ExecuteAsync(deleteClaimsSql, new
                    {
                        RoleId = role.Id
                    }, transaction);
                    const string insertClaimsSql = "INSERT INTO RoleClaims (RoleId, ClaimType, ClaimValue) " +
                                                   "VALUES (@RoleId, @ClaimType, @ClaimValue);";
                    await conn.ExecuteAsync(insertClaimsSql, claims.Select(x => new
                    {
                        RoleId = role.Id,
                        x.ClaimType,
                        x.ClaimValue
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
