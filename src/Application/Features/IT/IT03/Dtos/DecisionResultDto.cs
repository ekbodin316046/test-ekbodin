namespace Application.Features.IT.IT03.Dtos;

public record DecisionResultDto(
    int AffectedCount,
    IReadOnlyList<int> DocumentIds,
    string StatusNameTh);
