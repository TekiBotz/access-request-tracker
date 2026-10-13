using AccessTracker.Data;
using AccessTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccessTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    // Grants controller access to the database.
    private readonly AppDbContext _db;

    public RequestsController(AppDbContext db)
    {
        _db = db;
    }

    // User fields filled in when submitting a request
    public class NewRequest
    {
        public string RequesterName { get; set; } = "";
        public int ResourceId { get; set; }
        public string Reason { get; set; } = "";
    }

    // POST /api/requests...create a new request, starts as Pending
    [HttpPost]
    public IActionResult Submit(NewRequest input)
    {
        var request = new AccessRequest
        {
            RequesterName = input.RequesterName,
            ResourceId = input.ResourceId,
            Reason = input.Reason,
            Status = "Pending",
            CreatedAt = DateTime.Now
        };

        _db.AccessRequests.Add(request);
        _db.SaveChanges(); // save first so the request gets its Id

        AddAudit(request.Id, "Submitted", input.RequesterName);
        _db.SaveChanges();

        return Ok(request);
    }

    // GET /api/requests...list all requests, or only those with a given status
    [HttpGet]
    public IActionResult GetAll(string? status)
    {
        var query = _db.AccessRequests.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status == status);
        }

        return Ok(query.ToList());
    }

    // GET /api/requests/5...one request plus its history
    [HttpGet("{id}")]
    public IActionResult GetOne(int id)
    {
        var request = _db.AccessRequests.Find(id);
        if (request == null)
        {
            return NotFound();
        }

        var history = _db.AuditEntries
            .Where(a => a.AccessRequestId == id)
            .ToList();

        return Ok(new { request, history });
    }

    // POST /api/requests/5/approve
    [HttpPost("{id}/approve")]
    public IActionResult Approve(int id, string adminName)
    {
        return Decide(id, "Approved", adminName);
    }

    // POST /api/requests/5/deny
    [HttpPost("{id}/deny")]
    public IActionResult Deny(int id, string adminName)
    {
        return Decide(id, "Denied", adminName);
    }

    // Shared logic for approve and deny so we don't repeat ourselves. *************************************************************
    private IActionResult Decide(int id, string decision, string adminName)
    {
        var request = _db.AccessRequests.Find(id);
        if (request == null)
        {
            return NotFound();
        }

        request.Status = decision;
        request.DecidedAt = DateTime.Now;
        request.DecidedBy = adminName;

        AddAudit(request.Id, decision, adminName);
        _db.SaveChanges();

        return Ok(request);
    }

    // Adds a row to the history table.
    private void AddAudit(int requestId, string action, string actor)
    {
        var entry = new AuditEntry
        {
            AccessRequestId = requestId,
            Action = action,
            Actor = actor,
            Timestamp = DateTime.Now
        };
        _db.AuditEntries.Add(entry);
    }
}