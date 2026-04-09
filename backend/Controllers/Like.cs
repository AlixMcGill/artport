using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend.Extensions;

[ApiController]
[Route("api/[controller]")]
public class LikeController : ControllerBase
{
    private readonly LikeService _likeService;

    public LikeController(LikeService likeService)
    {
        _likeService = likeService;
    }

    // POST /api/like
    // Creates a like for a post by the authenticated user.
    [Authorize]
    [HttpPost("{id}")]
    public async Task<IActionResult> CreateLike(int id)
    {
        var userId = User.GetUserId();

        var likeCreated = await _likeService.CreateLikeAsync(id, userId);

        if (!likeCreated)
        {
            return BadRequest("Like already exists.");
        }

        return Ok();
    }

} 