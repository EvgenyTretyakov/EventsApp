using EventsWebApplication.Models;

namespace EventsWebApplication.Services.Interfaces
{
    public interface IBookingQueue
    {
        void Enqueue(Booking booking);
        bool TryDequeue(out Guid bookingId);
        Booking GetBooking(Guid id);
        bool AnyToProcess();
        void UpdateStatus(Guid id);
    }
}
