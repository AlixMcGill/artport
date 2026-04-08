using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly CommentService _commentService;
    public CommentController(CommentService commentService)
    {
        _commentService = commentService;
    }

    // GET /api/comment/{id}
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetComments(int id, [FromQuery] GetCommentsRequestDto request)
    {
        var comments = await _commentService.GetComments(id, request);

        if (comments == null) return NotFound();

        return Ok(comments);
    }

    // POST /api/comment
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequestDto request)
    {
        var comment = await _commentService.CreateComment(request);

        if (comment == null) return BadRequest();

        return Ok(comment);
    }
}