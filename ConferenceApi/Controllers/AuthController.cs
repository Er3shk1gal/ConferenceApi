using ConferenceApi.Models;
using ConferenceApi.Models.Database;
using ConferenceApi.Models.RequestModels;
using ConferenceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApi.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJWTService _jwtService;
    

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IJWTService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (await _userManager.FindByNameAsync(model.Username) != null)
        {
            return BadRequest("Username is already taken");
        }

        if (await _userManager.FindByEmailAsync(model.Email) != null)
        {
            return BadRequest("Email is already registered");
        }

        var user = new User 
        { 
            UserName = model.Username, 
            Email = model.Email,
            Fullname = model.Fullname,
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(new { Message = "Registration successful" });
        }
        
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user == null)
        {
            return Unauthorized("Invalid username or password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        
        if (result.Succeeded)
        {
            var token = await _jwtService.GenerateToken(user);
            return Ok(new { Token = token });
        }
        
        return Unauthorized("Invalid username or password");
    }
    
    
    [HttpGet("Test")]
    [Authorize()]
    public async Task<IActionResult> Test()
    {
        
        return Ok();
        
    }
    
}