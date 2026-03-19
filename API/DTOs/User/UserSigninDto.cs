using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.User;

public class UserSigninDto
{
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[^<>'\u0022]+$", ErrorMessage = "Invalid Special Characters")]
    public string Username { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[^<>'\u0022]+$", ErrorMessage = "Invalid Special Characters")]
    public string Password { get; set; } = string.Empty;
}
