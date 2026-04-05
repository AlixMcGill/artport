public class CreatePostRequestDto
{
    public string? Caption {get; set;}
    public IFormFile File {get; set;} = default!;
}