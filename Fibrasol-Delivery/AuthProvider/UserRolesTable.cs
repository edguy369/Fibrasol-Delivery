using Fibrasol_Delivery.AuthProvider.Abstract;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using Fibrasol_Delivery.Config;

namespace Fibrasol_Delivery.AuthProvider;

public class UserRolesTable<TRole, TKey, TUserRole> :
    IdentityTable,
    IUserRolesTable<TRole, TKey, TUserRole>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TUserRole : IdentityUserRole<TKey>, new()
{
    private readonly ConnectionString _connectionString;
    public UserRolesTable(ConnectionString connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }


    public virtual async Task<IEnumerable<TRole>> GetRolesAsync(TKey userId)
    {
        const string sql = "SELECT r.* " +
                           "FROM Roles AS r " +
                           "INNER JOIN UserRoles AS ur ON ur.RoleId = r.Id " +
                           "WHERE ur.UserId = @UserId;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var userRoles = await conn.QueryAsync<TRole>(sql, new { UserId = userId });
            return userRoles;
        }

    }

    public virtual async Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId)
    {
        const string sql = "SELECT * " +
                           "FROM UserRoles " +
                           "WHERE UserId = @UserId AND RoleId = @RoleId;";
        using (var conn = new MySqlConnection(_connectionString.Value))
        {
            var userRole = await conn.QuerySingleOrDefaultAsync<TUserRole>(sql, new
            {
                UserId = userId,
                RoleId = roleId
            });
            return userRole;
        }

    }
}
