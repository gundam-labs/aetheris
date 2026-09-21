using Aetheris.Web.Data;
using Aetheris.Web.Domain;
using Aetheris.Web.Tenancy;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aetheris.Web.Pages;

public class IndexModel : PageModel
{
    private readonly AetherisDbContext _db;
    private readonly CurrentUser _user;

    public IndexModel(AetherisDbContext db, CurrentUser user)
    {
        _db = db;
        _user = user;
    }

    public int OpenInbox { get; set; }
    public int TodayAppointments { get; set; }
    public int OpenEncounters { get; set; }
    public List<Appointment> Next { get; set; } = new();
    public List<Encounter> Board { get; set; } = new();

    public async Task OnGetAsync()
    {
        var tenant = _user.TenantId;
        var start = DateTimeOffset.Now.Date;
        var end = start.AddDays(1);
        OpenInbox = await _db.Inbox.CountAsync(x => x.TenantId == tenant && x.Status == "Open");
        TodayAppointments = await _db.Appointments.CountAsync(x => x.TenantId == tenant && x.StartsAt >= start && x.StartsAt < end);
        OpenEncounters = await _db.Encounters.CountAsync(x => x.TenantId == tenant && x.Status == "Open");
        Next = await _db.Appointments.Include(x => x.Patient)
            .Where(x => x.TenantId == tenant && x.StartsAt >= DateTimeOffset.Now)
            .OrderBy(x => x.StartsAt)
            .Take(5)
            .ToListAsync();
        Board = await _db.Encounters.Include(x => x.Patient)
            .Where(x => x.TenantId == tenant && x.Status == "Open")
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync();
    }
}
