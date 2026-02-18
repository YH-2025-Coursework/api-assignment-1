namespace Workshop.Api.Entities;

// Workshop entity tracked by EF Core.
public class Workshop
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int MaxParticipants { get; set; }

    // Populated when sessions are included; avoids null navigation collections.
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
