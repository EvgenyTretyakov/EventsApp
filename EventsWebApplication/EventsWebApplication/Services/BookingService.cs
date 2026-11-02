using EventsWebApplication.Enums;
using EventsWebApplication.Models;
using EventsWebApplication.Services.Interfaces;

namespace EventsWebApplication.Services
{
    public class BookingService : IBookingService
    {
        private readonly IEventService _eventService;
        private readonly IBookingQueue _bookingQueue;

        public BookingService(IEventService eventService, IBookingQueue bookingQueue)
        {
            _eventService = eventService;
            _bookingQueue = bookingQueue;
        }

        public async Task<Booking> CreateBookingAsync(Guid eventId)
        {
            var evnt = _eventService.GetEvent(eventId);

            await Task.Delay(1000);

            var newBooking = new Booking
            {
                Id = Guid.NewGuid(),
                EventId = evnt.Id,
                Status = BookingStatusEnum.Pending,
                CreatedAt = DateTime.Now
            };

            _bookingQueue.Enqueue(newBooking);

            return newBooking;
        }

        public async Task<Booking?> GetBookingByIdAsync(Guid bookingId)
        {
            await Task.Delay(1000);

            var booking = _bookingQueue.GetBooking(bookingId);

            return booking != null ? booking : null;
        }
    }
}
