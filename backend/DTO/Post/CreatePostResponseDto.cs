using System.Data;

public class CreatePostResponseDto
{
    public int Id {get; set;}
    public int UserId {get; set;} = default!;
    public string PhotoUrl {get; set;} = default;
    public string? Caption {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}