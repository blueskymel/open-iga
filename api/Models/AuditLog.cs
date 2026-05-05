namespace OpenIga.Api.Models;

public class AuditLog
{
    public Guid Id { get; set; }
    public AuditAction Action { get; set; }
    public Guid? PerformedBy { get; set; }
    public Guid? TargetUser { get; set; }
    public DateTime Timestamp { get; set; }
}
