using System;
using API.Models;

namespace API.DTOs.User;

public class UserDetailDto : BaseEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
