using EventsWebApplication.Models;

namespace EventsWebApplication.Services.Interfaces
{
    public interface IBookingQueue
    {
        void Enqueue(Booking booking);
        bool TryDequeue(out Booking booking);
        Booking GetBooking(Guid id);
        bool AnyToProcess();
    }
}
