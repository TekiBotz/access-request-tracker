namespace AccessTracker.Models;

// One request from a person asking for access to a resource.
public class AccessRequest
{
    public int Id { get; set; }
    public string RequesterName { get; set; } = "";
    public int ResourceId { get; set; }
    public string Reason { get; set; } = "";

    // Pending, Approved, or Denied
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }
    public DateTime? DecidedAt { get; set; }
    public string? DecidedBy { get; set; }
}