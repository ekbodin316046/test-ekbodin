using Application.Common.Interfaces;

namespace Api.Services;

public class CurrentUserAccessor : ICurrentUserAccessor
{
    private const string DefaultUserName = "demo.user";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // No authentication in scope, so the caller may name itself via a header.
    public string UserName
    {
        get
        {
            var header = _httpContextAccessor.HttpContext?.Request.Headers["X-User"].ToString();

            return string.IsNullOrWhiteSpace(header) ? DefaultUserName : header.Trim();
        }
    }
}
