using Aetheris.Web.Data;
using Aetheris.Web.Domain;
using Aetheris.Web.Tenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aetheris.Web.Pages.Patients;

public class IndexModel : PageModel
{
    private readonly AetherisDbContext _db;
    private readonly CurrentUser _user;

    public IndexModel(AetherisDbContext db, CurrentUser user)
    {
        _db = db;
        _user = user;
    }

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    public List<Patient> Results { get; set; } = new();

    public async Task OnGetAsync()
    {
        var query = _db.Patients.Where(x => x.TenantId == _user.TenantId);
        if (!string.IsNullOrWhiteSpace(Q))
        {
            var term = Q.Trim();
            query = query.Where(x =>
                x.Mrn.Contains(term) ||
                x.FamilyName.Contains(term) ||
                x.GivenName.Contains(term) ||
                (x.GivenName + " " + x.FamilyName).Contains(term));
        }

        Results = await query.OrderBy(x => x.FamilyName).ThenBy(x => x.GivenName).Take(50).ToListAsync();
    }
}
