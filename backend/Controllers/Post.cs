using System.ComponentModel.DataAnnotations;
using System.IO;
using Backend.Data;
using Backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Authorize]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;
    public PostController(PostService postService)
    {
        _postService = postService;
    }

    // Incoming POST body for creating a post.
    // `file` must be sent via multipart/form-data as an IFormFile.
    // `caption` is optional.
    public record CreatePostRequest(string? Caption);

    // GET api/post
    // Returns the latest posts sorted by creation date descending (newest first).
    // Optional `limit` query parameter controls number of posts (defaults to 20, max 100).
    [Authorize]
    [HttpGet("feed")]
    public async Task<IActionResult> Posts([FromQuery] GetPostsQueryDto query)
    {        
        var result = await _postService.GetFeedPostsAsync(query);

        if (result != null)
        {
            return Ok(result);
        } 
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }

    // GET api/post/user?userId={id}&limit={limit}
    // Returns the posts by the specified userId in newest-first order.
    // If userId is not provided, defaults to authenticated user.
    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> PostsByUser([FromQuery] GetPostsQueryDto query)
    {
        var targetUserId = User.GetUserId();

        var result = await _postService.GetPostsAsync(targetUserId, query);

        if (result != null)
        {
            return Ok(result);
        } 
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // POST api/post
    // Creates a new post for the authenticated user.
    // Must be multipart/form-data with:
    // - file: image file (required)
    // - caption: optional text
    // The server saves the file to wwwroot/uploads and sets photoUrl from the saved path.
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Posts([FromForm] CreatePostRequestDto request)
    {
        var userId = User.GetUserId();

        var result = await _postService.CreatePostAsync(userId, request);

        if (result != null)
        {
            return Ok();
        } 
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        //return Ok(result);
    }

}