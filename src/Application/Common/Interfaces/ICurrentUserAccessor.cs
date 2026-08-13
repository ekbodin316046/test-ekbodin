namespace Application.Common.Interfaces;

// Written into the created_by / updated_by and created_program / updated_program
// columns. The single seam where real authentication would plug in; the exam
// does not require it.
public interface ICurrentUserAccessor
{
    string UserName { get; }
    string ProgramCode { get; }
}
