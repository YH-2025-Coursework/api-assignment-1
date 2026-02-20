using System.ComponentModel.DataAnnotations;

namespace Workshop.Api.Dtos;

public class WorkshopCreateDto
{
    // Title must be provided and at least 3 chars.
    [Required]
    [MinLength(3)]
    public string Title { get; set; } = string.Empty;

    // Always require a description for create operations.
    [Required]
    public string Description { get; set; } = string.Empty;

    [FutureDate]
    public DateTime Date { get; set; }

    // Valid values must be between 1 and int.MaxValue (i.e., any positive int).
    [Range(1, int.MaxValue, ErrorMessage = "Max participants must be at least 1.")]
    public int MaxParticipants { get; set; }
}
