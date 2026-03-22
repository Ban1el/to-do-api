using System;

namespace API.DTOs.AudiTrail;

public class AuditTrailCreateDto
{
    public int UserId { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string ClientIpAddress { get; set; } = string.Empty;
    public bool IsRequest { get; set; }
    public string Path { get; set; } = string.Empty;
    public string RefId { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
}
