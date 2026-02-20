namespace Workshop.Api.Dtos;

public class WorkshopResponseDto
{
    // Response DTOs must expose the Id so clients can address that workshop later.
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int MaxParticipants { get; set; }

    // The description property is omitted. 
}
