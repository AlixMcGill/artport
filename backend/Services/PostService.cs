using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PostService
{
    private readonly ApplicationDbContext _context;
    private readonly FileStorageService _fileStorage;

    public PostService(ApplicationDbContext context, FileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<GetPostsResponseDto> GetFeedPostsAsync(GetPostsQueryDto query)
    {
        var posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .Take(query.Limit)
            .Select(p => new PostsDto
            {
                    Id = p.Id,
                    User = new UserDto
                    {
                        Id = p.UserId,
                        Username = p.User.Username,
                        ProfilePictureUrl = p.User.ProfilePictureUrl
                    },
                    PhotoUrl = p.PhotoUrl,
                    Caption = p.Caption,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    LikesCount = p.Likes.Count,
                    CommentsCount = p.Comments.Count
            }).ToListAsync();

            return new GetPostsResponseDto
            {
                Posts = posts
            };
    }

    public async Task<GetPostsResponseDto> GetPostsAsync(int targetUserId, GetPostsQueryDto query)
    { 

        // Rename this its specific to user posts only

            var posts = await _context.Posts
                .Where(p => p.UserId == targetUserId)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .Take(query.Limit)
                .Select(p => new PostsDto
                {
                    Id = p.Id,
                    User = new UserDto
                    {
                        Id = p.UserId,
                        Username = p.User.Username,
                        ProfilePictureUrl = p.User.ProfilePictureUrl
                    },
                    PhotoUrl = p.PhotoUrl,
                    Caption = p.Caption,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    LikesCount = p.Likes.Count,
                    CommentsCount = p.Comments.Count
                })
                .ToListAsync();

            return new GetPostsResponseDto
            {
                Posts = posts
            };
    }

    public async Task<CreatePostResponseDto> CreatePostAsync(int userId, CreatePostRequestDto request)
    {
        var photoUrl = await _fileStorage.Create(request.File); // Creates the file and returns the url

        var post = new Post // define data for writing to database
        {
            UserId = userId,
            PhotoUrl = photoUrl,
            Caption = string.IsNullOrWhiteSpace(request.Caption) ? null : request.Caption.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post); // Add to the database
        await _context.SaveChangesAsync(); // save changes to database

        return new CreatePostResponseDto // create data response
        {
            Id = post.Id,
            UserId = post.UserId,
            PhotoUrl = post.PhotoUrl,
            Caption = post.Caption,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}