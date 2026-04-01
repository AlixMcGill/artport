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
    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
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
        if (file == null || file.Length == 0) return BadRequest("No File");

        var userId = User.GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null) return NotFound();

        // Delete old file if exists and is still under uploads
        if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
        {
            var currentUrl = user.ProfilePictureUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var currentPath = Path.Combine("wwwroot", currentUrl);

            if (System.IO.File.Exists(currentPath))
            {
                try
                {
                    System.IO.File.Delete(currentPath);
                }
                catch
                {
                    // fail silently, we don't want upload to fail if delete can't happen
                }
            }
        }

        var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine("wwwroot/uploads", filename);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"/uploads/{filename}";
        user.ProfilePictureUrl = imageUrl;

        await _context.SaveChangesAsync();

        return Ok(new { profilePictureUrl = imageUrl });
    }
}