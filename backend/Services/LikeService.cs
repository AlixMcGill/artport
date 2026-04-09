using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

public class LikeService
{
    private readonly ApplicationDbContext _dbContext;

    public LikeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateLikeAsync(int postId, int userId)
    {
        var like = new Like
        {
            PostId = postId,
            UserId = userId
        };

        try
        {
            _dbContext.Likes.Add(like);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (DbUpdateException)
        {
            // Handle the case where the like already exists (unique constraint violation)
            return false;
        }

    }
}