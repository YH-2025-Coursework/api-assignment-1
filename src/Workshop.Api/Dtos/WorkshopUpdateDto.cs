using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Workshop.Api.Dtos;

public class WorkshopUpdateDto
{
    // Keep POST/PUT validation consistent: title is mandatory and >= 3 chars.
    [Required]
    [MinLength(3)]
    public string Title { get; set; } = string.Empty;

    // Description remains required even on update payloads.

    /*
    “Payload” refers to the data a client sends in an HTTP request body. In this context,
    the PUT endpoint expects a JSON body matching WorkshopUpdateDto, so that JSON is the update payload.
    */

    [Required]
    public string Description { get; set; } = string.Empty;

    [FutureDate]
    public DateTime Date { get; set; }

    // Valid values must be between 1 and int.MaxValue (i.e., any positive int).
    [Range(1, int.MaxValue, ErrorMessage = "Max participants must be at least 1.")]
    public int MaxParticipants { get; set; }
}
