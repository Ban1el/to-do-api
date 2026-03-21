using System;
using Newtonsoft.Json;

namespace API.DTOs.Buggy;

public class BuggyUpdateDto
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;
}
