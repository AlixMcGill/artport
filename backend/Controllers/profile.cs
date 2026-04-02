using Backend.Data;
using Backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly FileStorageService _fileStorage;
    public ProfileController(ApplicationDbContext context, FileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        // Get user id from the token
        var userId = User.GetUserId();

        var user = await _context.Users
        .Where(u => u.Id == userId)
        .Select(u => new
        {
            username = u.Username,
            bio = u.Bio,
            profilePictureUrl = u.ProfilePictureUrl
        }).FirstOrDefaultAsync();

        if (user == null) return NotFound();

        return Ok(user);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] GetProfile dto)
    {
        var userId = User.GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null) return NotFound();

        user.Username = dto.Username;
        user.Bio = dto.Bio;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            username = user.Username,
            bio = user.Bio,
            profilePictureUrl = user.ProfilePictureUrl
        });
    }

    [Authorize]
    [HttpPost("update/profile-image")]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        var userId = User.GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null) return NotFound();

        var imageUrl = await _fileStorage.Update(file, user.ProfilePictureUrl);
        user.ProfilePictureUrl = imageUrl;

        await _context.SaveChangesAsync();

        return Ok(new { profilePictureUrl = imageUrl });
    }
}