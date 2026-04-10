using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class PostService
{
    private readonly ApplicationDbContext _context;
    private readonly FileStorageService _fileStorage;
    private readonly IMemoryCache _cache;

    public PostService(ApplicationDbContext context, FileStorageService fileStorage, IMemoryCache cache)
    {
        _context = context;
        _fileStorage = fileStorage;
        _cache = cache;
    }

    public async Task<GetPostsResponseDto> GetFeedPostsAsync(GetPostsQueryDto query)
    {
        List<PostsDto> posts;

        switch (query.Sort)
        {
            case "trending":
                posts = await GetTrendingFeedAsync(query);
                break;
            default:
                posts = await GetLastestFeedAsync(query);
                break;
        }

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
                .Take(query.PageSize)
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

    private async Task<List<PostsDto>> GetLastestFeedAsync(GetPostsQueryDto query)
    {
        const int maxCachedPosts = 100;
        const int cacheExperationInMinutes = 10;

        int skip = (query.Page - 1) * query.PageSize;

        if (skip >= maxCachedPosts)
        {
            return await QueryLastestFeedFromDbAsync(query);
        }

        var cacheKey = "trending:top100";

        var cached = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExperationInMinutes);

            return await QueryLastestFeedFromDbAsync(new GetPostsQueryDto
            {
                Page = 1,
                PageSize = maxCachedPosts
            });
        });

        return (cached ?? new List<PostsDto>())
            .Skip(skip)
            .Take(query.PageSize)
            .ToList();
    }
    private async Task<List<PostsDto>> QueryLastestFeedFromDbAsync(GetPostsQueryDto query)
    {
        var posts = await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.Page -1) * query.PageSize)
            .Take(query.PageSize)
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

        return posts;
    }

    private async Task<List<PostsDto>> GetTrendingFeedAsync(GetPostsQueryDto query)
    {
        const int maxCachedPosts = 100;
        const int cacheExperationInMinutes = 10;

        int skip = (query.Page - 1) * query.PageSize;

        if (skip >= maxCachedPosts)
        {
            return await QueryTrendingFeedFromDbAsync(query);
        }

        var cacheKey = "trending:top100";

        var cached = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExperationInMinutes);

            return await QueryTrendingFeedFromDbAsync(new GetPostsQueryDto
            {
                Page = 1,
                PageSize = maxCachedPosts
            });
        });

        return (cached ?? new List<PostsDto>())
            .Skip(skip)
            .Take(query.PageSize)
            .ToList();
    }
    private async Task<List<PostsDto>> QueryTrendingFeedFromDbAsync(GetPostsQueryDto query)
    {
        var posts = await _context.Posts
            .OrderByDescending(p => p.Comments.Count)
            .ThenByDescending(p => p.Likes.Count)
            .Skip((query.Page -1) * query.PageSize)
            .Take(query.PageSize)
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

            return posts;
    }
}