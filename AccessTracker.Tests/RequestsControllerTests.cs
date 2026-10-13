using AccessTracker.Controllers;
using AccessTracker.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AccessTracker.Tests;

public class RequestsControllerTests
{
    // In-memory database for each test, so tests don't affect each other.
    private AppDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void Submit_CreatesPendingRequest_AndAuditEntry()
    {
        var db = NewDb();
        var controller = new RequestsController(db);

        controller.Submit(new RequestsController.NewRequest
        {
            RequesterName = "Test User",
            ResourceId = 1,
            Reason = "Testing"
        });

        var request = Assert.Single(db.AccessRequests);
        Assert.Equal("Pending", request.Status);

        var audit = Assert.Single(db.AuditEntries);
        Assert.Equal("Submitted", audit.Action);
    }

    [Fact]
    public void Approve_SetsStatusToApproved()
    {
        var db = NewDb();
        var controller = new RequestsController(db);

        controller.Submit(new RequestsController.NewRequest
        {
            RequesterName = "Test User",
            ResourceId = 1,
            Reason = "Testing"
        });
        var id = db.AccessRequests.Single().Id;

        controller.Approve(id, "Admin");

        var request = db.AccessRequests.Single();
        Assert.Equal("Approved", request.Status);
        Assert.Equal("Admin", request.DecidedBy);
    }
}