namespace Application.Features.IT.IT03.Dtos;

public record DocumentListItemDto(
    int Id,
    string DocumentName,
    string? Reason,
    int StatusId,
    string StatusCode,
    string StatusNameTh,
    bool IsPending,
    DateTime CreatedAt,
    DateTime UpdatedAt);
