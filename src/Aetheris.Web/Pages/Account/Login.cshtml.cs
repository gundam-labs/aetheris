using System.Security.Claims;
using Aetheris.Web.Data;
using Aetheris.Web.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aetheris.Web.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly AetherisDbContext _db;

    public LoginModel(AetherisDbContext db) => _db = db;

    [BindProperty] public string Email { get; set; } = "maya.rao@harborview.demo";
    [BindProperty] public string Password { get; set; } = "";
    public string? Error { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string? returnUrl)
    {
        var user = await _db.Users.Include(x => x.Tenant)
            .FirstOrDefaultAsync(x => x.Email == Email);
        if (user is null || !Passwords.Verify(user.PasswordHash, Password))
        {
            Error = "Unknown staff account or wrong password.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("name", user.DisplayName),
            new("uid", user.Id.ToString()),
            new("tenant", user.TenantId.ToString()),
            new("tenantName", user.Tenant?.Name ?? "")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
    }
}
