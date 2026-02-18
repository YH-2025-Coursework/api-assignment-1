namespace Workshop.Api.Entities;

// Entity for individual workshop sessions.
public class Session
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // FK + navigation back to the parent workshop.
    public Guid WorkshopId { get; set; }
    public Workshop? Workshop { get; set; }
}
