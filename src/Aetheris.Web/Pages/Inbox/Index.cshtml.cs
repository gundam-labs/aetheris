using Aetheris.Web.Data;
using Aetheris.Web.Domain;
using Aetheris.Web.Tenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aetheris.Web.Pages.Inbox;

public class IndexModel : PageModel
{
    private readonly AetherisDbContext _db;
    private readonly CurrentUser _user;

    public IndexModel(AetherisDbContext db, CurrentUser user)
    {
        _db = db;
        _user = user;
    }

    public List<InboxItem> Items { get; set; } = new();

    public async Task OnGetAsync()
    {
        Items = await _db.Inbox.Include(x => x.Patient)
            .Where(x => x.TenantId == _user.TenantId && x.Status == "Open")
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDoneAsync(Guid id)
    {
        var item = await _db.Inbox.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _user.TenantId);
        if (item is not null)
        {
            item.Status = "Done";
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
