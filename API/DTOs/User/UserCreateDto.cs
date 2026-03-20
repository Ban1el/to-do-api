using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.User;

public class UserCreateDto
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string password { get; set; } = string.Empty;
}
