using CommunityGuard.Api.Models;
using CommunityGuard.Api.Services;
using CommunityGuard.Shared.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = CommunityGuard.Shared.Dtos.LoginRequest;

namespace CommunityGuard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(Shared.Dtos.LoginRequest request)
    {
        // 1. Find the user by email.
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Unauthorized("Invalid credentials.");

        // 2. Check the password.
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return Unauthorized("Invalid credentials.");

        // 3. Get the user's role.
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Responder";

        // 4. Generate the JWT.
        var token = _tokenService.CreateToken(user, role);

        // 5. Return the token to the client.
        return Ok(new LoginResponse(token, user.Email!, role));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(LoginRequest request)
    {
        // 1. Create the new user object.
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = "Test User" };

        // 2. Save to database (passwords are automatically hashed by Identity).
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        // 3. Assign the "Controller" role so they have full access.
        await _userManager.AddToRoleAsync(user, "Controller");

        return Ok("User created successfully!");
    }
}