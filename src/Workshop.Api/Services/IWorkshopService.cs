using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workshop.Api.Dtos;

namespace Workshop.Api.Services;

// Contract for workshop CRUD/search operations so controllers stay thin.
public interface IWorkshopService
{
    Task<IReadOnlyList<WorkshopResponseDto>> GetAllAsync(string? search, CancellationToken cancellationToken = default);
    Task<WorkshopResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkshopResponseDto> CreateAsync(WorkshopCreateDto createDto, CancellationToken cancellationToken = default);
    Task<WorkshopResponseDto?> UpdateAsync(Guid id, WorkshopUpdateDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

// A CancellationToken travels with async operations so callers can signal "stop" (for example when a request is aborted).

/*
The ? on return types (e.g., Task<WorkshopResponseDto?>) tells callers the method might produce null. For GetByIdAsync
and UpdateAsync, that happens when the requested workshop doesn’t exist - returning null lets the controller detect “not
found” without throwing. The other methods (CreateAsync) always succeed with a real DTO, so no nullable marker there.
The DTO parameters themselves (e.g., WorkshopUpdateDto updateDto) aren’t nullable; DI/ModelBinding should provide
validated instances.
*/