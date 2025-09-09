using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    #region Views
    [Route("login")]
    public IActionResult Index()
    {
        return View();
    }
    #endregion


    #region HTTP Methods
    [HttpPost]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Unauthorized();

        var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, true, false);
        if (!result.Succeeded)
            return Unauthorized();

        return Ok();
    }
    #endregion
}
