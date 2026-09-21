using Aetheris.Web.Data;
using Aetheris.Web.Domain;
using Aetheris.Web.Tenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aetheris.Web.Pages.Chart;

public class IndexModel : PageModel
{
    private readonly AetherisDbContext _db;
    private readonly CurrentUser _user;

    public IndexModel(AetherisDbContext db, CurrentUser user)
    {
        _db = db;
        _user = user;
    }

    public Patient? Patient { get; set; }
    [BindProperty] public string Type { get; set; } = EncounterTypes.Outpatient;
    [BindProperty] public string Location { get; set; } = "Clinic A";
    [BindProperty] public string? ChiefComplaint { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Patient = await Load(id);
        return Patient is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostStartAsync(Guid id)
    {
        var patient = await Load(id);
        if (patient is null)
        {
            return NotFound();
        }

        _db.Encounters.Add(new Encounter
        {
            Id = Guid.NewGuid(),
            TenantId = _user.TenantId,
            PatientId = id,
            Type = Type,
            Status = "Open",
            StartedAt = DateTimeOffset.Now,
            Location = Location,
            Clinician = _user.DisplayName,
            ChiefComplaint = ChiefComplaint
        });
        await _db.SaveChangesAsync();
        return Redirect($"/Chart/{id}");
    }

    private Task<Patient?> Load(Guid id) =>
        _db.Patients
            .Include(x => x.Allergies)
            .Include(x => x.Problems)
            .Include(x => x.Medications)
            .Include(x => x.Encounters)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _user.TenantId);
}
