using igrwijaya.Net.Identity.MongoDB.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace igrwijaya.Net.Identity.Test.API.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<MongoIdentityRole> _roleManager;

    // GET
    public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<MongoIdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var user = new ApplicationUser("igrwijaya", "igrwijaya@test.com");
        var result = await _userManager.CreateAsync(user, "Temp123*");

        await _userManager.AddToRoleAsync(user, "ADMIN");

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> CreateRole()
    {
        await _roleManager.CreateAsync(new MongoIdentityRole("ADMIN"));
        
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        var user = await _userManager.FindByNameAsync("igrwijaya");

        var result = await _signInManager.PasswordSignInAsync(user, "Temp123*", false, false);

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(result);
    }
}