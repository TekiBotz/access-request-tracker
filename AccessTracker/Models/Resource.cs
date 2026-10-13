namespace AccessTracker.Models;

// Something people can request access to, like a shared drive or an app.
public class Resource
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}