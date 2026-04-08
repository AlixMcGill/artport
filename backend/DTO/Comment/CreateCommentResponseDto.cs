public class CreateCommentResponseDto
{
    public int CommentId {get; set;}
    public int UserId {get; set;}
    public int PostId {get; set;}
    public string Content {get; set;} = default!;
    public DateTime CreatedAt {get; set;}
}