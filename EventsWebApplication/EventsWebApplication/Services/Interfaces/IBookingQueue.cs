using EventsWebApplication.Models;

namespace EventsWebApplication.Services.Interfaces
{
    public interface IBookingQueue
    {
        void Enqueue(Booking booking);
        bool TryDequeue(out Guid bookingId);
        Booking GetBooking(Guid id);
        void UpdateStatus(Guid id);
        void Update(Booking booking);
        IEnumerable<Booking> GetPending();
    }
}
