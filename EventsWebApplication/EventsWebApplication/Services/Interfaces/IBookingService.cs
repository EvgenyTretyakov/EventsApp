using EventsWebApplication.Models;

namespace EventsWebApplication.Services.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> CreateBookingAsync(Guid eventId);
        Task<Booking?> GetBookingByIdAsync(Guid bookingId);
    }
}
