using System.ComponentModel.DataAnnotations;
using System.IO;
using Backend.Data;
using Backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Authorize]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public PostController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Incoming POST body for creating a post.
    // `file` must be sent via multipart/form-data as an IFormFile.
    // `caption` is optional.
    public record CreatePostRequest(string? Caption);

    // GET api/post
    // Returns the latest posts sorted by creation date descending (newest first).
    // Optional `limit` query parameter controls number of posts (defaults to 20, max 100).
    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> Posts([FromQuery] int limit = 20)
    {
        if (limit <= 0 || limit > 100) limit = 20;

        var posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .Select(p => new
            {
                id = p.Id,
                user = new
                {
                    id = p.UserId,
                    username = p.User.Username,
                    profilePictureUrl = p.User.ProfilePictureUrl
                },
                photoUrl = p.PhotoUrl,
                caption = p.Caption,
                createdAt = p.CreatedAt,
                updatedAt = p.UpdatedAt,
                likesCount = p.Likes.Count,
                commentsCount = p.Comments.Count
            })
            .ToListAsync();

        return Ok(posts);
    }

    // GET api/post/user?userId={id}&limit={limit}
    // Returns the posts by the specified userId in newest-first order.
    // If userId is not provided, defaults to authenticated user.
    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> PostsByUser([FromQuery] int? userId, [FromQuery] int limit = 20)
    {
        if (limit <= 0 || limit > 100) limit = 20;

        var targetUserId = userId ?? User.GetUserId();

        var posts = await _context.Posts
            .Where(p => p.UserId == targetUserId)
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .Select(p => new
            {
                id = p.Id,
                user = new
                {
                    id = p.UserId,
                    username = p.User.Username,
                    profilePictureUrl = p.User.ProfilePictureUrl
                },
                photoUrl = p.PhotoUrl,
                caption = p.Caption,
                createdAt = p.CreatedAt,
                updatedAt = p.UpdatedAt,
                likesCount = p.Likes.Count,
                commentsCount = p.Comments.Count
            })
            .ToListAsync();

        return Ok(posts);
    }

    // POST api/post
    // Creates a new post for the authenticated user.
    // Must be multipart/form-data with:
    // - file: image file (required)
    // - caption: optional text
    // The server saves the file to wwwroot/uploads and sets photoUrl from the saved path.
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Posts([FromForm] CreatePostRequest request, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "Image file is required" });

        // Could validate file type/size here (jpg/png etc.)
        var uploadsFolder = Path.Combine("wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var localPath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(localPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var photoUrl = $"/uploads/{fileName}";

        var userId = User.GetUserId();

        var post = new Backend.Models.Post
        {
            UserId = userId,
            PhotoUrl = photoUrl,
            Caption = string.IsNullOrWhiteSpace(request.Caption) ? null : request.Caption.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Posts), new { id = post.Id }, new
        {
            id = post.Id,
            userId = post.UserId,
            photoUrl = post.PhotoUrl,
            caption = post.Caption,
            createdAt = post.CreatedAt,
            updatedAt = post.UpdatedAt
        });
    }

}