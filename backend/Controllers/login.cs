using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly ApplicationDbContext _context;

    private readonly JwtService _jwtService;

    public AuthController(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        request.Email = request.Email.Trim().ToLower();
        request.Username = request.Username.Trim().ToLower();

        // Search the database for any users that already contain the username and or email.
        var exists = await _context.Users.AnyAsync(u =>
        u.Email == request.Email || u.Username == request.Username);

        // return a bad request if the user already exists.
        if (exists) return BadRequest("Email or Username already taken.");

        // create the new user to be written to the database
        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            PassHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        
        return Ok(new
        {
            message = "User created succesfully",
            userId = user.Id,
            token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // fail safe to make sure request exists
        if (request == null) return BadRequest("Invalid Request");

        // Grab the user from the database through the application db context
        // returns the first user provieded
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        // check to make sure the is a user returned otherwise invalidate the request
        if (user == null) return Unauthorized("Invalid Credentials");

        // validate that the password is correct
        bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PassHash);

        // if the password is incorrect request is not valid
        if (!isValid) return Unauthorized("Invalid Credentials");

        // generate jwt token for future authorization
        var token = _jwtService.GenerateToken(user);

        // username and password were correct. Request successful
        return Ok(new
        {
            token,
            userId = user.Id
        });
    }
}
