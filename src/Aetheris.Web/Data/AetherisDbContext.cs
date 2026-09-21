using Aetheris.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aetheris.Web.Data;

public sealed class AetherisDbContext : DbContext
{
    public AetherisDbContext(DbContextOptions<AetherisDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<StaffUser> Users => Set<StaffUser>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Allergy> Allergies => Set<Allergy>();
    public DbSet<Problem> Problems => Set<Problem>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<InboxItem> Inbox => Set<InboxItem>();
    public DbSet<LabOrder> LabOrders => Set<LabOrder>();
    public DbSet<Observation> Observations => Set<Observation>();
    public DbSet<InterfaceMessage> InterfaceMessages => Set<InterfaceMessage>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Tenant>().HasIndex(x => x.Slug).IsUnique();
        model.Entity<StaffUser>().HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
        model.Entity<Patient>().HasIndex(x => new { x.TenantId, x.Mrn }).IsUnique();
        model.Entity<Patient>()
            .HasMany(x => x.Allergies).WithOne().HasForeignKey(x => x.PatientId);
        model.Entity<Patient>()
            .HasMany(x => x.Problems).WithOne().HasForeignKey(x => x.PatientId);
        model.Entity<Patient>()
            .HasMany(x => x.Medications).WithOne().HasForeignKey(x => x.PatientId);
        model.Entity<Encounter>()
            .HasOne(x => x.Patient).WithMany(x => x.Encounters).HasForeignKey(x => x.PatientId);
        model.Entity<Appointment>()
            .HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId);
        model.Entity<InboxItem>()
            .HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId);
        model.Entity<LabOrder>()
            .HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId);
        model.Entity<LabOrder>()
            .HasIndex(x => new { x.TenantId, x.PlacerOrderNumber }).IsUnique();
        model.Entity<Observation>()
            .HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId);
        model.Entity<InterfaceMessage>()
            .HasIndex(x => new { x.TenantId, x.ControlId, x.Direction });
    }
}
