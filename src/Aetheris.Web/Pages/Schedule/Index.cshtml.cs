using Aetheris.Web.Data;
using Aetheris.Web.Domain;
using Aetheris.Web.Tenancy;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aetheris.Web.Pages.Schedule;

public class IndexModel : PageModel
{
    private readonly AetherisDbContext _db;
    private readonly CurrentUser _user;

    public IndexModel(AetherisDbContext db, CurrentUser user)
    {
        _db = db;
        _user = user;
    }

    public List<Appointment> Day { get; set; } = new();

    public async Task OnGetAsync()
    {
        var start = DateTimeOffset.Now.Date;
        var end = start.AddDays(1);
        Day = await _db.Appointments.Include(x => x.Patient)
            .Where(x => x.TenantId == _user.TenantId && x.StartsAt >= start && x.StartsAt < end)
            .OrderBy(x => x.StartsAt)
            .ToListAsync();
    }
}
