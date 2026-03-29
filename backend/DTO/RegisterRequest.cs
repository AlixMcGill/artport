using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    required public string Email {get; set;}
    [Required]
    [MinLength(8)]
    required public string Password {get; set;}
    [Required]
    [MinLength(4)]
    [MaxLength(20)]
    required public string Username {get; set;}
}