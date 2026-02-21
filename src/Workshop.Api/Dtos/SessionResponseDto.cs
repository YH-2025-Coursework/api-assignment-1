namespace Workshop.Api.Dtos;

// Shape returned to clients when they interact with the session endpoints.
public sealed class SessionResponseDto
{
    public Guid Id { get; set; }
    public Guid WorkshopId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
