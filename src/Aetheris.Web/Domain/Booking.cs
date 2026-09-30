namespace Aetheris.Web.Domain;

public static class AppointmentStatuses
{
    public const string Booked = "Booked";
    public const string Arrived = "Arrived";
    public const string Cancelled = "Cancelled";
}

public static class Clinics
{
    public static readonly string[] Names =
    [
        "Primary Care 2",
        "Women's Health",
        "Oncology clinic",
        "Paediatrics",
        "Urgent Care",
    ];
}

public static class Booking
{
    public static string? SlotConflict(
        IEnumerable<Appointment> existing,
        DateTimeOffset startsAt,
        string clinic,
        Guid? exceptId = null)
    {
        var taken = existing.Any(a =>
            a.Id != exceptId
            && a.Clinic == clinic
            && a.StartsAt == startsAt
            && a.Status is AppointmentStatuses.Booked or AppointmentStatuses.Arrived);
        return taken ? "That slot is already booked in this clinic." : null;
    }

    public static Appointment Hold(
        Guid tenantId,
        Guid patientId,
        DateTimeOffset startsAt,
        string clinic,
        string provider,
        string reason)
    {
        return new Appointment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            PatientId = patientId,
            StartsAt = startsAt,
            DurationMinutes = 20,
            Clinic = clinic,
            Provider = provider,
            Status = AppointmentStatuses.Booked,
            Reason = reason
        };
    }
}
