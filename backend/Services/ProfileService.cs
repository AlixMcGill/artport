using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProfileService
{
    private readonly ApplicationDbContext _context;
    private readonly FileStorageService _fileStorage;

    public ProfileService(ApplicationDbContext context, FileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<GetProfile> getProfileData(int userId)
    {
        var user = await _context.Users
        .Where(u => u.Id == userId)
        .Select(u => new GetProfile
        {
            Username = u.Username,
            Bio = u.Bio,
            ProfilePictureUrl = u.ProfilePictureUrl
        }).FirstAsync();

        return user;
    }

    public async Task<GetProfile> UpdateProfile(int userId, GetProfile dto)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null) throw new Exception("Could not find User.");

        user.Username = dto.Username;
        user.Bio = dto.Bio;

        await _context.SaveChangesAsync();

        return new GetProfile
        {
            Username = user.Username,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    public async Task<string> WriteProfileImage(IFormFile file, int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null) throw new Exception("Could not find user");

        var imageUrl = await _fileStorage.Update(file, user.ProfilePictureUrl);

        if (imageUrl == null) throw new Exception("Could not write new file");

        user.ProfilePictureUrl = imageUrl;

        await _context.SaveChangesAsync();

        return imageUrl;
    }
}