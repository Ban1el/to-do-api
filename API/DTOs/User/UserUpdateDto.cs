using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.User;

public class UserUpdateDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
}
