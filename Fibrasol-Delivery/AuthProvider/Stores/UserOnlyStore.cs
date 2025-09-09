using Fibrasol_Delivery.AuthProvider.Abstract;
using Fibrasol_Delivery.AuthProvider.Includes;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Fibrasol_Delivery.AuthProvider.Stores
{
    public class UserOnlyStore<TUser, TKey, TUserClaim, TUserLogin, TUserToken> :
        UserStoreBase<TUser, TKey, TUserClaim, TUserLogin, TUserToken>,
        IProtectedUserStore<TUser>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        public UserOnlyStore(IUsersOnlyTable<TUser, TKey, TUserClaim, TUserLogin, TUserToken> usersTable, IUserClaimsTable<TKey, TUserClaim> userClaimsTable, IUserLoginsTable<TUser, TKey, TUserLogin> userLoginsTable,
            IUserTokensTable<TKey, TUserToken> userTokensTable, IdentityErrorDescriber describer) : base(describer)
        {
            UsersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
            UserClaimsTable = userClaimsTable ?? throw new ArgumentNullException(nameof(userClaimsTable));
            UserLoginsTable = userLoginsTable ?? throw new ArgumentNullException(nameof(userLoginsTable));
            UserTokensTable = userTokensTable ?? throw new ArgumentNullException(nameof(userTokensTable));
        }

        private IList<TUserClaim> UserClaims { get; set; }
        private IList<TUserLogin> UserLogins { get; set; }
        private IList<TUserToken> UserTokens { get; set; }
        public IUsersOnlyTable<TUser, TKey, TUserClaim, TUserLogin, TUserToken> UsersTable { get; }
        public IUserClaimsTable<TKey, TUserClaim> UserClaimsTable { get; }
        public IUserLoginsTable<TUser, TKey, TUserLogin> UserLoginsTable { get; }
        public IUserTokensTable<TKey, TUserToken> UserTokensTable { get; }
        public override IQueryable<TUser> Users => throw new NotSupportedException();
        public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            claims.ThrowIfNull(nameof(claims));
            UserClaims ??= (await UserClaimsTable.GetClaimsAsync(user.Id)).ToList();
            foreach (var claim in claims)
            {
                UserClaims.Add(CreateUserClaim(user, claim));
            }
        }
        public override async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            login.ThrowIfNull(nameof(login));
            UserLogins ??= (await UserLoginsTable.GetLoginsAsync(user.Id)).ToList();
            UserLogins.Add(CreateUserLogin(user, login));
        }
        public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var created = await UsersTable.CreateAsync(user);
            return created ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"User '{user.UserName}' could not be created."
            });
        }
        public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var deleted = await UsersTable.DeleteAsync(user.Id);
            return deleted ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"User '{user.UserName}' could not be deleted."
            });
        }
        public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var user = await UsersTable.FindByEmailAsync(normalizedEmail);
            return user;
        }
        public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var id = ConvertIdFromString(userId);
            var user = await UsersTable.FindByIdAsync(id);
            return user;
        }
        public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var user = await UsersTable.FindByNameAsync(normalizedUserName);
            return user;
        }
        public override async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var userClaims = await UserClaimsTable.GetClaimsAsync(user.Id);
            return userClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
        }
        public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var userLogins = await UserLoginsTable.GetLoginsAsync(user.Id);
            return userLogins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList();
        }
        public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            claim.ThrowIfNull(nameof(claim));
            var users = await UsersTable.GetUsersForClaimAsync(claim);
            return users.ToList();
        }
        public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            claims.ThrowIfNull(nameof(claims));
            UserClaims ??= (await UserClaimsTable.GetClaimsAsync(user.Id)).ToList();
            foreach (var claim in claims)
            {
                var matchedClaims = UserClaims.Where(x => x.UserId.Equals(user.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
                foreach (var matchedClaim in matchedClaims)
                {
                    UserClaims.Remove(matchedClaim);
                }
            }
        }
        public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            UserLogins ??= (await UserLoginsTable.GetLoginsAsync(user.Id)).ToList();
            var userLogin = await FindUserLoginAsync(user.Id, loginProvider, providerKey, cancellationToken);
            if (userLogin != null)
            {
                UserLogins.Remove(userLogin);
            }
        }
        public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            claim.ThrowIfNull(nameof(claim));
            newClaim.ThrowIfNull(nameof(newClaim));
            UserClaims ??= (await UserClaimsTable.GetClaimsAsync(user.Id)).ToList();
            var matchedClaims = UserClaims.Where(x => x.UserId.Equals(user.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            foreach (var matchedClaim in matchedClaims)
            {
                matchedClaim.ClaimValue = newClaim.Value;
                matchedClaim.ClaimType = newClaim.Type;
            }
        }
        public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            var updated = await UsersTable.UpdateAsync(user, UserClaims, UserLogins, UserTokens);
            return updated ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"User '{user.UserName}' could not be deleted."
            });
        }
        protected override async Task AddUserTokenAsync(TUserToken token)
        {
            token.ThrowIfNull(nameof(token));
            UserTokens ??= (await UserTokensTable.GetTokensAsync(token.UserId)).ToList();
            UserTokens.Add(token);
        }
        protected override async Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var token = await UserTokensTable.FindTokenAsync(user.Id, loginProvider, name);
            return token;
        }
        protected override async Task<TUser> FindUserAsync(TKey userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var user = await UsersTable.FindByIdAsync(userId);
            return user;
        }
        protected override async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await UserLoginsTable.FindUserLoginAsync(loginProvider, providerKey);
            return userLogin;
        }
        protected override async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await UserLoginsTable.FindUserLoginAsync(loginProvider, providerKey);
            return userLogin;
        }
        protected override async Task RemoveUserTokenAsync(TUserToken token)
        {
            UserTokens ??= (await UserTokensTable.GetTokensAsync(token.UserId)).ToList();
            UserTokens.Remove(token);
        }
    }
}
