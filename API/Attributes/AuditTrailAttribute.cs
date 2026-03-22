namespace API.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AuditTrailAttribute : Attribute
{
    public string Module { get; set; } = "Unknown";
    public string Action { get; set; } = "Unknown";
}