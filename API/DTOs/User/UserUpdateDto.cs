using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.User;

public class UserUpdateDto
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
