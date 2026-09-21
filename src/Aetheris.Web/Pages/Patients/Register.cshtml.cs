using Aetheris.Web.Data;
using Aetheris.Web.Domain;
using Aetheris.Web.Tenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aetheris.Web.Pages.Patients;

public class RegisterModel : PageModel
{
    private readonly AetherisDbContext _db;
    private readonly CurrentUser _user;

    public RegisterModel(AetherisDbContext db, CurrentUser user)
    {
        _db = db;
        _user = user;
    }

    [BindProperty] public string GivenName { get; set; } = "";
    [BindProperty] public string FamilyName { get; set; } = "";
    [BindProperty] public DateOnly DateOfBirth { get; set; } = new(1980, 1, 1);
    [BindProperty] public string Sex { get; set; } = "F";
    [BindProperty] public string? Phone { get; set; }
    public string? Error { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(GivenName) || string.IsNullOrWhiteSpace(FamilyName))
        {
            Error = "Given name and family name are required.";
            return Page();
        }

        var last = await _db.Patients.Where(x => x.TenantId == _user.TenantId)
            .OrderByDescending(x => x.Mrn)
            .Select(x => x.Mrn)
            .FirstOrDefaultAsync();
        var seq = 100001;
        if (last is not null)
        {
            var tail = last.Split('-').LastOrDefault();
            if (int.TryParse(tail, out var n))
            {
                seq = n + 1;
            }
        }

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            TenantId = _user.TenantId,
            Mrn = Mrn.Next("HV", seq),
            GivenName = GivenName.Trim(),
            FamilyName = FamilyName.Trim(),
            DateOfBirth = DateOfBirth,
            Sex = Sex,
            Phone = Phone,
            PrimaryLanguage = "English"
        };
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();
        return Redirect($"/Chart/{patient.Id}");
    }
}
