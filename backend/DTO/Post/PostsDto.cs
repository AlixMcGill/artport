public class PostsDto
{
    public int Id {get; set;}
    public UserDto User {get; set;} = default!;

    public string PhotoUrl {get; set;} = default!;
    public string? Caption {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    public int LikesCount {get; set;}
    public int CommentsCount {get; set;}
}