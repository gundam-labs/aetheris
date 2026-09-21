using System.Security.Claims;

namespace Aetheris.Web.Tenancy;

public sealed class CurrentUser
{
    public CurrentUser(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        IsAuthenticated = true;
        Email = user.Identity.Name ?? "";
        DisplayName = user.FindFirst("name")?.Value ?? Email;
        Role = user.FindFirst(ClaimTypes.Role)?.Value ?? "";
        if (Guid.TryParse(user.FindFirst("tenant")?.Value, out var tenantId))
        {
            TenantId = tenantId;
        }

        if (Guid.TryParse(user.FindFirst("uid")?.Value, out var userId))
        {
            UserId = userId;
        }

        TenantName = user.FindFirst("tenantName")?.Value ?? "";
    }

    public bool IsAuthenticated { get; }
    public Guid TenantId { get; }
    public Guid UserId { get; }
    public string Email { get; } = "";
    public string DisplayName { get; } = "";
    public string Role { get; } = "";
    public string TenantName { get; } = "";
}
