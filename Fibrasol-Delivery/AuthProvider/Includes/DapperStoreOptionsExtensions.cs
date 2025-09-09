using Fibrasol_Delivery.AuthProvider;
using Fibrasol_Delivery.AuthProvider.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Fibrasol_Delivery.AuthProvider.Includes;

public static class DapperStoreOptionsExtensions
{
    public static void AddRoleClaimsTable<TRoleClaimsTable, TRoleClaim>(this DapperStoreOptions options)
        where TRoleClaimsTable : RoleClaimsTable<string, TRoleClaim>
        where TRoleClaim : IdentityRoleClaim<string>, new()
    {
        options.AddRoleClaimsTable<TRoleClaimsTable, string, TRoleClaim>();
    }

    public static void AddRoleClaimsTable<TRoleClaimsTable, TKey, TRoleClaim>(this DapperStoreOptions options)
        where TRoleClaimsTable : RoleClaimsTable<TKey, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        options.Services.AddScoped(typeof(IRoleClaimsTable<,>).MakeGenericType(typeof(TKey), typeof(TRoleClaim)), typeof(TRoleClaimsTable));
    }

    public static void AddRolesTable<TRolesTable, TRole>(this DapperStoreOptions options)
        where TRolesTable : RolesTable<TRole, string, IdentityRoleClaim<string>>
        where TRole : IdentityRole<string>
    {
        options.AddRolesTable<TRolesTable, TRole, string, IdentityRoleClaim<string>>();
    }

    public static void AddRolesTable<TRolesTable, TRole, TKey, TRoleClaim>(this DapperStoreOptions options)
        where TRolesTable : RolesTable<TRole, TKey, TRoleClaim>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        options.Services.AddScoped(typeof(IRolesTable<,,>).MakeGenericType(typeof(TRole), typeof(TKey), typeof(TRoleClaim)), typeof(TRolesTable));
    }

    public static void AddUserClaimsTable<TUserClaimsTable, TUserClaim>(this DapperStoreOptions options)
        where TUserClaimsTable : UserClaimsTable<string, TUserClaim>
        where TUserClaim : IdentityUserClaim<string>, new()
    {
        options.AddUserClaimsTable<TUserClaimsTable, string, TUserClaim>();
    }

    public static void AddUserClaimsTable<TUserClaimsTable, TKey, TUserClaim>(this DapperStoreOptions options)
        where TUserClaimsTable : UserClaimsTable<TKey, TUserClaim>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
    {
        options.Services.AddScoped(typeof(IUserClaimsTable<,>).MakeGenericType(typeof(TKey), typeof(TUserClaim)), typeof(TUserClaimsTable));
    }

    public static void AddUserLoginsTable<TUserLoginsTable, TUserLogin>(this DapperStoreOptions options)
        where TUserLoginsTable : UserLoginsTable<IdentityUser, string, TUserLogin>
        where TUserLogin : IdentityUserLogin<string>, new()
    {
        options.AddUserLoginsTable<TUserLoginsTable, IdentityUser, string, TUserLogin>();
    }

    public static void AddUserLoginsTable<TUserLoginsTable, TUser, TKey, TUserLogin>(this DapperStoreOptions options)
        where TUserLoginsTable : UserLoginsTable<TUser, TKey, TUserLogin>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
    {
        options.Services.AddScoped(typeof(IUserLoginsTable<,,>).MakeGenericType(typeof(TUser), typeof(TKey), typeof(TUserLogin)), typeof(TUserLoginsTable));
    }

    public static void AddUserRolesTable<TUserRolesTable, TUserRole>(this DapperStoreOptions options)
        where TUserRolesTable : UserRolesTable<IdentityRole, string, TUserRole>
        where TUserRole : IdentityUserRole<string>, new()
    {
        options.AddUserRolesTable<TUserRolesTable, IdentityRole, string, TUserRole>();
    }

    public static void AddUserRolesTable<TUserRolesTable, TRole, TKey, TUserRole>(this DapperStoreOptions options)
        where TUserRolesTable : UserRolesTable<TRole, TKey, TUserRole>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
    {
        options.Services.AddScoped(typeof(IUserRolesTable<,,>).MakeGenericType(typeof(TRole), typeof(TKey), typeof(TUserRole)), typeof(TUserRolesTable));
    }

    public static void AddUsersTable<TUsersTable, TUser>(this DapperStoreOptions options)
        where TUsersTable : UsersTable<TUser, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>>
        where TUser : IdentityUser<string>
    {
        options.AddUsersTable<TUsersTable, TUser, string>();
    }

    public static void AddUsersTable<TUsersTable, TUser, TKey>(this DapperStoreOptions options)
        where TUsersTable : UsersTable<TUser, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityUserToken<TKey>>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        options.AddUsersTable<TUsersTable, TUser, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityUserToken<TKey>>();
    }

    public static void AddUsersTable<TUsersTable, TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>(this DapperStoreOptions options)
        where TUsersTable : UsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        options.Services.AddScoped(typeof(IUsersTable<,,,,,>).MakeGenericType(typeof(TUser), typeof(TKey), typeof(TUserClaim), typeof(TUserRole), typeof(TUserLogin), typeof(TUserToken)), typeof(TUsersTable));
    }

    public static void AddUserTokensTable<TUserTokensTable, TUserToken>(this DapperStoreOptions options)
        where TUserTokensTable : UserTokensTable<string, TUserToken>
        where TUserToken : IdentityUserToken<string>, new()
    {
        options.AddUserTokensTable<TUserTokensTable, string, TUserToken>();
    }

    public static void AddUserTokensTable<TUserTokensTable, TKey, TUserToken>(this DapperStoreOptions options)
        where TUserTokensTable : UserTokensTable<TKey, TUserToken>
        where TKey : IEquatable<TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        options.Services.AddScoped(typeof(IUserTokensTable<,>).MakeGenericType(typeof(TKey), typeof(TUserToken)), typeof(TUserTokensTable));
    }
}
