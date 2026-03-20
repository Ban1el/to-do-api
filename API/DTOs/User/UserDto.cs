using System.Text.Json.Serialization;
using API.Models;

namespace API.DTOs.User;

public class UserDto : BaseEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;
    [JsonIgnore]
    public string PasswordSalt { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
