using Application.Common.Messaging;
using Application.Features.IT.IT03.Common;
using Application.Features.IT.IT03.Dtos;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Features.IT.IT03;

public static class Approve
{
    public record Command : ICommand<DecisionResultDto>
    {
        public List<int> DocumentIds { get; init; } = [];
        public string Reason { get; init; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.DocumentIds)
                .NotEmpty().WithMessage("กรุณาเลือกรายการที่ต้องการอนุมัติ");

            RuleFor(command => command.Reason)
                .NotEmpty().WithMessage("กรุณากรอกเหตุผล")
                .MaximumLength(500).WithMessage("เหตุผลต้องไม่เกิน 500 ตัวอักษร");
        }
    }

    public class Handler : IRequestHandler<Command, DecisionResultDto>
    {
        private readonly DocumentDecisionExecutor _executor;

        public Handler(DocumentDecisionExecutor executor)
        {
            _executor = executor;
        }

        public Task<DecisionResultDto> Handle(Command request, CancellationToken cancellationToken) =>
            _executor.ExecuteAsync(
                request.DocumentIds,
                DocumentStatusCode.Approved,
                request.Reason,
                cancellationToken);
    }
}
