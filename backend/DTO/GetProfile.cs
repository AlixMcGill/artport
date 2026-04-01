using System.ComponentModel.DataAnnotations;

public class GetProfile
{
    [Required]
    [MinLength(4)]
    [MaxLength(20)]
    required public string Username {get; set;}

    [MaxLength(500)]
    public string? Bio {get; set;}

    [MaxLength(500)]
    public string? ProfilePictureUrl {get; set;}
}