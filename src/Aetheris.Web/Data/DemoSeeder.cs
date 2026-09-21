using Aetheris.Web.Domain;
using Aetheris.Web.Security;
using Microsoft.EntityFrameworkCore;

namespace Aetheris.Web.Data;

public static class DemoSeeder
{
    public static async Task SeedAsync(AetherisDbContext db)
    {
        if (await db.Tenants.AnyAsync())
        {
            return;
        }

        var tenantId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeee0001");
        db.Tenants.Add(new Tenant
        {
            Id = tenantId,
            Slug = "harborview",
            Name = "Harborview General",
            TimeZone = "Africa/Lagos"
        });

        db.Users.AddRange(
            Staff("maya.rao@harborview.demo", "Maya Rao, MD", StaffRoles.Attending, "Clinician#2026"),
            Staff("james.okoro@harborview.demo", "James Okoro, RN", StaffRoles.Nurse, "Nurse#2026"),
            Staff("priya.shah@harborview.demo", "Priya Shah", StaffRoles.Registrar, "Registrar#2026"));

        var elena = Patient(1, "Elena", "Vasquez", new DateOnly(1978, 4, 12), "F", "+1-555-0101");
        var samuel = Patient(2, "Samuel", "Okonkwo", new DateOnly(1959, 11, 3), "M", "+1-555-0102");
        var aisha = Patient(3, "Aisha", "Mensah", new DateOnly(2020, 6, 18), "F", "+1-555-0103");
        var robert = Patient(4, "Robert", "Chen", new DateOnly(1955, 1, 29), "M", "+1-555-0104");
        var fatima = Patient(5, "Fatima", "Al-Najjar", new DateOnly(1992, 8, 7), "F", "+1-555-0105");
        var david = Patient(6, "David", "Park", new DateOnly(1997, 2, 14), "M", "+1-555-0106");
        var grace = Patient(7, "Grace", "Nwosu", new DateOnly(1971, 9, 22), "F", "+1-555-0107");
        var tomas = Patient(8, "Tomas", "Alvarez", new DateOnly(1944, 12, 1), "M", "+1-555-0108");

        db.Patients.AddRange(elena, samuel, aisha, robert, fatima, david, grace, tomas);

        db.Allergies.AddRange(
            new Allergy { Id = Guid.NewGuid(), PatientId = elena.Id, Substance = "Lisinopril", Reaction = "Angioedema", Criticality = "High" },
            new Allergy { Id = Guid.NewGuid(), PatientId = robert.Id, Substance = "Penicillin", Reaction = "Rash", Criticality = "Low" });

        db.Problems.AddRange(
            new Problem { Id = Guid.NewGuid(), PatientId = elena.Id, Display = "Asthma", Status = "Active", Onset = new DateOnly(2004, 1, 1) },
            new Problem { Id = Guid.NewGuid(), PatientId = samuel.Id, Display = "Type 2 diabetes mellitus", Status = "Active", Onset = new DateOnly(2012, 5, 1) },
            new Problem { Id = Guid.NewGuid(), PatientId = samuel.Id, Display = "Essential hypertension", Status = "Active", Onset = new DateOnly(2010, 3, 1) },
            new Problem { Id = Guid.NewGuid(), PatientId = robert.Id, Display = "Heart failure with reduced EF", Status = "Active", Onset = new DateOnly(2018, 11, 1) },
            new Problem { Id = Guid.NewGuid(), PatientId = fatima.Id, Display = "Pregnancy, 28 weeks", Status = "Active", Onset = new DateOnly(2026, 3, 10) },
            new Problem { Id = Guid.NewGuid(), PatientId = grace.Id, Display = "Breast cancer, in remission", Status = "Active", Onset = new DateOnly(2023, 2, 1) },
            new Problem { Id = Guid.NewGuid(), PatientId = tomas.Id, Display = "Community-acquired pneumonia", Status = "Active", Onset = new DateOnly(2026, 9, 18) });

        db.Medications.AddRange(
            new Medication { Id = Guid.NewGuid(), PatientId = elena.Id, Name = "Budesonide-formoterol inhaler", Sig = "2 puffs BID", Status = "Active" },
            new Medication { Id = Guid.NewGuid(), PatientId = samuel.Id, Name = "Metformin 1000 mg", Sig = "1 tab BID with food", Status = "Active" },
            new Medication { Id = Guid.NewGuid(), PatientId = samuel.Id, Name = "Amlodipine 10 mg", Sig = "1 tab daily", Status = "Active" },
            new Medication { Id = Guid.NewGuid(), PatientId = robert.Id, Name = "Warfarin 5 mg", Sig = "as directed by INR", Status = "Active" },
            new Medication { Id = Guid.NewGuid(), PatientId = robert.Id, Name = "Furosemide 40 mg", Sig = "1 tab daily", Status = "Active" });

        var today = DateTimeOffset.Now.Date;
        db.Appointments.AddRange(
            Appt(elena, today.AddHours(9), "Primary Care 2", "Maya Rao, MD", "Asthma review"),
            Appt(samuel, today.AddHours(9).AddMinutes(20), "Primary Care 2", "Maya Rao, MD", "Diabetes follow-up"),
            Appt(fatima, today.AddHours(10), "Women's Health", "Maya Rao, MD", "Antenatal visit"),
            Appt(grace, today.AddHours(11), "Oncology clinic", "Maya Rao, MD", "Surveillance"),
            Appt(aisha, today.AddHours(14), "Paediatrics", "Maya Rao, MD", "Well child"));

        db.Encounters.AddRange(
            new Encounter
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PatientId = david.Id,
                Type = EncounterTypes.Emergency,
                Status = "Open",
                StartedAt = DateTimeOffset.Now.AddHours(-2),
                Location = "ED Bay 4",
                Clinician = "James Okoro, RN",
                ChiefComplaint = "Ankle injury after football"
            },
            new Encounter
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PatientId = tomas.Id,
                Type = EncounterTypes.Inpatient,
                Status = "Open",
                StartedAt = DateTimeOffset.Now.AddDays(-3),
                Location = "Ward 3B / Bed 12",
                Clinician = "Maya Rao, MD",
                ChiefComplaint = "Fever and productive cough"
            });

        db.Inbox.AddRange(
            new InboxItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PatientId = samuel.Id,
                Type = InboxTypes.Result,
                Title = "HbA1c 8.4% — review",
                Body = "Drawn 18 Sep 2026. Prior 7.9%.",
                Status = "Open",
                AssignedRole = StaffRoles.Attending,
                CreatedAt = DateTimeOffset.Now.AddHours(-6)
            },
            new InboxItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PatientId = robert.Id,
                Type = InboxTypes.Result,
                Title = "INR 3.6 — high",
                Body = "Warfarin patient. Hold and call.",
                Status = "Open",
                AssignedRole = StaffRoles.Attending,
                CreatedAt = DateTimeOffset.Now.AddHours(-3)
            },
            new InboxItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PatientId = elena.Id,
                Type = InboxTypes.Refill,
                Title = "Refill: budesonide-formoterol",
                Body = "Patient requested 90-day refill via portal.",
                Status = "Open",
                AssignedRole = StaffRoles.Attending,
                CreatedAt = DateTimeOffset.Now.AddHours(-1)
            },
            new InboxItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PatientId = fatima.Id,
                Type = InboxTypes.PatientMessage,
                Title = "Portal: reduced fetal movement?",
                Body = "Message from patient this morning. Needs same-day advice.",
                Status = "Open",
                AssignedRole = StaffRoles.Nurse,
                CreatedAt = DateTimeOffset.Now.AddMinutes(-40)
            });

        await db.SaveChangesAsync();
        return;

        StaffUser Staff(string email, string name, string role, string password) => new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            DisplayName = name,
            Role = role,
            PasswordHash = Passwords.Hash(password)
        };

        Patient Patient(int seq, string given, string family, DateOnly dob, string sex, string phone) => new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Mrn = Mrn.Next("HV", 100000 + seq),
            GivenName = given,
            FamilyName = family,
            DateOfBirth = dob,
            Sex = sex,
            Phone = phone,
            PrimaryLanguage = "English"
        };

        Appointment Appt(Patient p, DateTimeOffset starts, string clinic, string provider, string reason) => new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            PatientId = p.Id,
            StartsAt = starts,
            DurationMinutes = 20,
            Clinic = clinic,
            Provider = provider,
            Status = "Booked",
            Reason = reason
        };
    }
}
