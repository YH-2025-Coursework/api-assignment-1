using System.ComponentModel.DataAnnotations;
using Workshop.Api.Dtos;

namespace Workshop.Api.Tests;

// Exercises the custom validation attributes applied to Workshop DTOs.
public class WorkshopValidationTests
{
    // FutureDateAttribute should flag a validation error when the date is in the past.
    [Fact]
    public void FutureDateAttribute_RejectsPastDates()
    {
        var dto = new WorkshopCreateDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Date = DateTime.UtcNow.AddDays(-1),
            MaxParticipants = 5
        };

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(validationResults, vr => vr.ErrorMessage == "Date must be today or in the future.");
    }

    // The same attribute should allow today/future timestamps so valid payloads pass.
    [Fact]
    public void FutureDateAttribute_AllowsTodayAndFutureDates()
    {
        var dto = new WorkshopCreateDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Date = DateTime.UtcNow.AddHours(1),
            MaxParticipants = 5
        };

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, validateAllProperties: true);

        Assert.True(isValid);
    }
}

/*
`Assert` comes from the xUnit testing framework. It contains static helper methods (like Assert.Equal, Assert.True,
Assert.Contains) that can be called inside tests to verify expected outcomes. If an assertion fails, xUnit marks the test
as failed and reports the mismatch; if all assertions pass, the test succeeds.
*/