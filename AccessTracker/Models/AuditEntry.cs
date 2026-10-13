namespace AccessTracker.Models;

// A history record of something that happened to a request.
public class AuditEntry
{
    public int Id { get; set; }
    public int AccessRequestId { get; set; }

    // Submitted, Approved, or Denied
    public string Action { get; set; } = "";

    public string Actor { get; set; } = "";
    public DateTime Timestamp { get; set; }
}