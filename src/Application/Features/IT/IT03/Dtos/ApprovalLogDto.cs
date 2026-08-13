namespace Application.Features.IT.IT03.Dtos;

public record ApprovalLogDto(
    int Id,
    int DocumentId,
    string FromStatusNameTh,
    string ToStatusNameTh,
    string Reason,
    string ActionBy,
    DateTime ActionAt);
