using Backend.Models;

public class GetCommentsResponseDto
{
    public List<CommentDto> Comments {get; set;} = new();
}