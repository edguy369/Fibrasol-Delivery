﻿using Microsoft.AspNetCore.Identity;

namespace Fibrasol_Delivery.AuthProvider.Abstract
{
    public interface IUserTokensTable<TKey, TUserToken>
        where TKey : IEquatable<TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        Task<IEnumerable<TUserToken>> GetTokensAsync(TKey userId);
        Task<TUserToken> FindTokenAsync(TKey userId, string loginProvider, string name);
    }
}
