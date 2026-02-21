using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace Workshop.Api.Dtos;

/*
Shared validation for create/update session payloads.

Sessions have both create and update payloads that share identical validation rules (title, future start, end-after-start),
so SessionBaseDto centralizes that logic once and lets SessionCreateDto/SessionUpdateDto inherit it.

IValidatableObject is part of .NET’s base class library. It lets a class provide custom validation logic beyond per-property attributes.
*/

public abstract class SessionBaseDto : IValidatableObject
{
    [Required]
    [MinLength(3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    /*
    `Validate` is the method defined by IValidatableObject. ASP.NET (via DataAnnotations) calls it during model validation,
    passing a ValidationContext. This implementation returns zero or more `ValidationResult` instances describing any
    problems. It returns IEnumerable<ValidationResult> so it's possible to yield multiple errors (e.g., one for StartTime,
    another for EndTime). If the sequence is empty, validation succeeds; if it contains results, ASP.NET turns them into
    ModelState errors.
    */

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime < DateTime.UtcNow)
        {
            yield return new ValidationResult("Start time must be in the future.", new[] { nameof(StartTime) });
        }

        if (EndTime <= StartTime)
        {
            yield return new ValidationResult("End time must be after the start time.", new[] { nameof(EndTime) });
        }
    }
}
