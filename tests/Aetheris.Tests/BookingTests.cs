using Aetheris.Web.Domain;

namespace Aetheris.Tests;

public class BookingTests
{
    [Fact]
    public void Blocks_same_clinic_and_time()
    {
        var slot = DateTimeOffset.Parse("2026-09-30T09:00:00+00:00");
        var existing = new[]
        {
            Booking.Hold(Guid.NewGuid(), Guid.NewGuid(), slot, "Primary Care 2", "Maya Rao, MD", "Review"),
        };
        Assert.Equal("That slot is already booked in this clinic.", Booking.SlotConflict(existing, slot, "Primary Care 2"));
    }

    [Fact]
    public void Allows_same_time_in_another_clinic()
    {
        var slot = DateTimeOffset.Parse("2026-09-30T09:00:00+00:00");
        var existing = new[]
        {
            Booking.Hold(Guid.NewGuid(), Guid.NewGuid(), slot, "Primary Care 2", "Maya Rao, MD", "Review"),
        };
        Assert.Null(Booking.SlotConflict(existing, slot, "Urgent Care"));
    }

    [Fact]
    public void Cancelled_slot_can_be_reused()
    {
        var slot = DateTimeOffset.Parse("2026-09-30T11:00:00+00:00");
        var held = Booking.Hold(Guid.NewGuid(), Guid.NewGuid(), slot, "Oncology clinic", "Maya Rao, MD", "Surveillance");
        held.Status = AppointmentStatuses.Cancelled;
        Assert.Null(Booking.SlotConflict([held], slot, "Oncology clinic"));
    }
}
