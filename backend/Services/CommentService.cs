using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

public class CommentService
{
    private readonly ApplicationDbContext _dbContext;
    public CommentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCommentsResponseDto> GetComments(int postId, GetCommentsRequestDto request)
    {
        var comments = await _dbContext.Comments
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CommentDto
            {
                CommentId = c.Id,
                UserId = c.UserId,
                PostId = c.PostId,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return new GetCommentsResponseDto()
        {
            Comments = comments
        };
    }

    public async Task<CreateCommentResponseDto> CreateComment(CreateCommentRequestDto request)
    {
        var comment = new Comment
        {
            UserId = request.UserId,
            PostId = request.PostId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();

        return new CreateCommentResponseDto
        {
            CommentId = comment.Id,
            UserId = comment.UserId,
            PostId = comment.PostId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }
}