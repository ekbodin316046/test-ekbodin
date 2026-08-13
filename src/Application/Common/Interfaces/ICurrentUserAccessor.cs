namespace Application.Common.Interfaces;

// Recorded as ApprovalLog.ActionBy. The single seam where real authentication
// would plug in; the exam does not require it.
public interface ICurrentUserAccessor
{
    string UserName { get; }
}
