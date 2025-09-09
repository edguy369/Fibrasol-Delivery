using Microsoft.AspNetCore.Identity;

namespace Fibrasol_Delivery.AuthProvider.Extensions;

public class ExtendedIdentityUser : IdentityUser<string>
{
    public string Name { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public bool IsContaEnabled { get; set; } = default!;
    public ExtendedIdentityUser() { }
}
