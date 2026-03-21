using System;

namespace API.DTOs.ErrorLog;

public class ErrorLogCreateDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ClientIpAddress { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
}
