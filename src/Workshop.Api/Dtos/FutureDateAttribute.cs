using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Workshop.Api.Dtos;

// Validates that a DateTime value is now or later.

/*
The attribute metadata constrains how [FutureDate] can be used:
  - `AttributeTargets.Property | AttributeTargets.Field` means it can only be applied to properties or fields. It couldn’t be
    attached to a class, method, etc.
  - `AllowMultiple = false` means it can only be applied once per property/field. If it were true, I could stack multiple
    [FutureDate] attributes with different settings.
*/

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]

// Deriving from ValidationAttribute lets me plug into ASP.NET Core’s model validation system.
public sealed class FutureDateAttribute : ValidationAttribute
{

    //  ValidationAttribute has a constructor that accepts a message string; by calling `base("Date must be today or in the
    // future.")`, I set ErrorMessage up front without hardcoding it in multiple places.
    public FutureDateAttribute() : base("Date must be today or in the future.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        // Compare only the date portion so a time earlier today still counts as valid.
        if (value is DateTime date && date.Date >= DateTime.UtcNow.Date)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage);
    }
}
