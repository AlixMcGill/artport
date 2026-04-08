public class CreateCommentRequestDto
{
    public int UserId {get; set;} = default!;
    public int PostId {get; set;}
    public string Content {get; set;} = default!;
}