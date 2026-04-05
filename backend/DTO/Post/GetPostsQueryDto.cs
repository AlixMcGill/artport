public class GetPostsQueryDto
{
    public string? Sort {get; set;}
    public int Page {get; set;} = 1;
    public int PageSize {get; set;} = 20;
}