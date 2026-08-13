using Application.Common.Interfaces;

namespace Api.Services;

public class CurrentUserAccessor : ICurrentUserAccessor
{
    private const string DefaultUserName = "demo.user";
    private const string DefaultProgramCode = "IT03";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // No authentication in scope, so the caller may name itself via a header.
    public string UserName => HeaderOr("X-User", DefaultUserName);

    // The screen that owns the request. A second screen writing these tables
    // would send its own code without any change here.
    public string ProgramCode => HeaderOr("X-Program", DefaultProgramCode);

    private string HeaderOr(string name, string fallback)
    {
        var header = _httpContextAccessor.HttpContext?.Request.Headers[name].ToString();

        return string.IsNullOrWhiteSpace(header) ? fallback : header.Trim();
    }
}
