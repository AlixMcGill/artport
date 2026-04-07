using Backend.Data;
using Backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ProfileService _profileService;
    public ProfileController(ProfileService profileService)
    {
        _profileService = profileService;
    }

    // GET /api/profile
    // Return the users profile data for dashboard
    // takes jwt
    // returns username, bio, and url for profile image
    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult<GetProfile>> Profile()
    {
        var userId = User.GetUserId();
        var user = await _profileService.getProfileData(userId);
        if (user == null) return NotFound();
        return Ok(user);
    }

    // PUT /api/profile
    // takes jwt and updated username, and bio
    // returns updated username, and bio for frontend to validate data
    [Authorize]
    [HttpPut("")]
    public async Task<ActionResult<GetProfile>> Update([FromBody] GetProfile dto)
    {
        // Prob not a good idea to use the same dto for input and output
        var userId = User.GetUserId();
        var updatedUser = await _profileService.UpdateProfile(userId, dto);
        if (updatedUser == null) return NotFound();
        return updatedUser;
    }

    // POST /api/profile/image
    // takes jwt and new profile image
    // writes new profile image to wwwroot/uploads, removes old profile image from directory
    // returns new url for profile image
    [Authorize]
    [HttpPost("profile-image")]
    //   ^    could not decern between POST and PUT
    public async Task<ActionResult<ProfileImageUrlResponseDto>> UploadProfileImage(IFormFile file)
    {
        var userId = User.GetUserId();
        var imageUrl = _profileService.WriteProfileImage(file, userId);
        if (imageUrl == null) return NotFound();
        return Ok(new { ProfilePictureUrl = imageUrl });
    }
}