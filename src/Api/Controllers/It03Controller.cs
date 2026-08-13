using Application.Features.IT.IT03;
using Application.Features.IT.IT03.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/it03")]
public class It03Controller : ControllerBase
{
    private readonly IMediator _mediator;

    public It03Controller(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("documents")]
    public async Task<ActionResult<IReadOnlyList<DocumentListItemDto>>> GetDocuments(
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetDocumentList.Query(), cancellationToken));
    }

    [HttpPost("documents/approve")]
    public async Task<ActionResult<DecisionResultDto>> Approve(
        Approve.Command command,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpPost("documents/reject")]
    public async Task<ActionResult<DecisionResultDto>> Reject(
        Reject.Command command,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpGet("statuses")]
    public async Task<ActionResult<IReadOnlyList<DocumentStatusDto>>> GetStatuses(
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetStatusList.Query(), cancellationToken));
    }

    [HttpGet("documents/{id:int}/logs")]
    public async Task<ActionResult<IReadOnlyList<ApprovalLogDto>>> GetApprovalHistory(
        int id,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetApprovalHistory.Query(id), cancellationToken));
    }
}
