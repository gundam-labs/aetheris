using Aetheris.Web.Data;
using Aetheris.Web.Tenancy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Error");
});

builder.Services.AddDbContext<AetherisDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Aetheris") ?? "Data Source=aetheris.db"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.Cookie.Name = "aetheris.station";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AetherisDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DemoSeeder.SeedAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.MapPost("/api/v1/auth/login", async (LoginRequest body, AetherisDbContext db) =>
{
    var user = await db.Users.Include(x => x.Tenant)
        .FirstOrDefaultAsync(x => x.Email == body.Email);
    if (user is null || !Aetheris.Web.Security.Passwords.Verify(user.PasswordHash, body.Password))
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new
    {
        user.Email,
        user.DisplayName,
        user.Role,
        tenant = user.Tenant?.Slug
    });
}).AllowAnonymous();

app.Run();

public sealed record LoginRequest(string Email, string Password);

public partial class Program;
