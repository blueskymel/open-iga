namespace OpenIga.Api.Models;

public enum AttestationStatus
{
    Pending,
    Approved,
    Revoked
}

public class Attestation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid ReviewerId { get; set; }
    public AttestationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
