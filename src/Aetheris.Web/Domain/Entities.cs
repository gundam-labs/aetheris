namespace Aetheris.Web.Domain;

public sealed class Tenant
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = "";
    public string Name { get; set; } = "";
    public string TimeZone { get; set; } = "UTC";
}

public static class StaffRoles
{
    public const string Attending = "Attending";
    public const string Nurse = "Nurse";
    public const string Registrar = "Registrar";
    public const string FrontDesk = "FrontDesk";
}

public sealed class StaffUser
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Role { get; set; } = StaffRoles.Nurse;
    public string PasswordHash { get; set; } = "";
    public Tenant? Tenant { get; set; }
}

public sealed class Patient
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Mrn { get; set; } = "";
    public string GivenName { get; set; } = "";
    public string FamilyName { get; set; } = "";
    public DateOnly DateOfBirth { get; set; }
    public string Sex { get; set; } = "";
    public string? Phone { get; set; }
    public string? PrimaryLanguage { get; set; }
    public ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();
    public ICollection<Problem> Problems { get; set; } = new List<Problem>();
    public ICollection<Medication> Medications { get; set; } = new List<Medication>();
    public ICollection<Encounter> Encounters { get; set; } = new List<Encounter>();

    public string FullName => $"{FamilyName}, {GivenName}";
}

public sealed class Allergy
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Substance { get; set; } = "";
    public string Reaction { get; set; } = "";
    public string Criticality { get; set; } = "Low";
}

public sealed class Problem
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Display { get; set; } = "";
    public string Status { get; set; } = "Active";
    public DateOnly? Onset { get; set; }
}

public sealed class Medication
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Name { get; set; } = "";
    public string Sig { get; set; } = "";
    public string Status { get; set; } = "Active";
}

public static class EncounterTypes
{
    public const string Outpatient = "Outpatient";
    public const string Inpatient = "Inpatient";
    public const string Emergency = "Emergency";
}

public sealed class Encounter
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PatientId { get; set; }
    public string Type { get; set; } = EncounterTypes.Outpatient;
    public string Status { get; set; } = "Open";
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public string Location { get; set; } = "";
    public string Clinician { get; set; } = "";
    public string? ChiefComplaint { get; set; }
    public Patient? Patient { get; set; }
}

public sealed class Appointment
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PatientId { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public int DurationMinutes { get; set; } = 20;
    public string Clinic { get; set; } = "";
    public string Provider { get; set; } = "";
    public string Status { get; set; } = "Booked";
    public string? Reason { get; set; }
    public Patient? Patient { get; set; }
}

public static class InboxTypes
{
    public const string Result = "Result";
    public const string Refill = "Refill";
    public const string PatientMessage = "PatientMessage";
    public const string StaffTask = "StaffTask";
}

public sealed class InboxItem
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? PatientId { get; set; }
    public string Type { get; set; } = InboxTypes.StaffTask;
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string Status { get; set; } = "Open";
    public string AssignedRole { get; set; } = StaffRoles.Attending;
    public DateTimeOffset CreatedAt { get; set; }
    public Patient? Patient { get; set; }
}

public static class Mrn
{
    public static string Next(string prefix, int sequence) =>
        $"{prefix}-{sequence:000000}";

    public static bool LooksValid(string value) =>
        !string.IsNullOrWhiteSpace(value) && value.Contains('-') && value.Length >= 5;
}
