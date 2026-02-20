using System.Collections.Generic;

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
    // The default instantiation happens whenever a Workshop object is constructed, so the collection starts as an empty list instead of null.
    // Entity Framework will later replace that collection(or fill it) when it materializes related Session rows,
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
