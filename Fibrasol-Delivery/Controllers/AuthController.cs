using Fibrasol_Delivery.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fibrasol_Delivery.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
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
    [Route("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {Email}", request.Email);

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email: {Email}", request.Email);
                return Unauthorized();
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, true, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: Invalid password for user: {Email}", request.Email);
                return Unauthorized();
            }

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while attempting to login user: {Email}", request.Email);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            _logger.LogInformation("User logout initiated");
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out successfully");
            return RedirectToAction("Index", "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while attempting to logout");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    #endregion
}
