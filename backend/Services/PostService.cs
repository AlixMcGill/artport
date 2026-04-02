using Backend.Data;
using Backend.Models;

public class PostService
{
    private readonly ApplicationDbContext _context;
    private readonly FileStorageService _fileStorage;

    public PostService(ApplicationDbContext context, FileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<CreatePostResponseDto> CreatePostAsync(int userId, CreatePostRequestDto request)
    {
        var photoUrl = await _fileStorage.Create(request.File);

        var post = new Post
        {
            UserId = userId,
            PhotoUrl = photoUrl,
            Caption = string.IsNullOrWhiteSpace(request.Caption) ? null : request.Caption.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post); // Add to the database
        await _context.SaveChangesAsync(); // save changes to database

        return new CreatePostResponseDto
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