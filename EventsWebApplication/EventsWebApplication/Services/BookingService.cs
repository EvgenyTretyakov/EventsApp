using EventsWebApplication.Enums;
using EventsWebApplication.Models;
using EventsWebApplication.Exceptions;
using EventsWebApplication.Services.Interfaces;

namespace EventsWebApplication.Services
{
    public class BookingService : IBookingService
    {
        private readonly IEventService _eventService;
        private readonly IBookingQueue _bookingQueue;

        private readonly object _bookingLock = new object();

        public BookingService(IEventService eventService, IBookingQueue bookingQueue)
        {
            _eventService = eventService;
            _bookingQueue = bookingQueue;
        }

        public async Task<Booking> CreateBookingAsync(Guid eventId)
        {
            await Task.Delay(0);

            var evnt = _eventService.GetEvent(eventId);

            lock (_bookingLock)
            {
                if (!evnt.TryReserveSeats())
                {
                    throw new NoAvailableSeatsException("No available seats for this event");
                }
            }

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
            await Task.Delay(0);

            var booking = _bookingQueue.GetBooking(bookingId);

            return booking != null ? booking : null;
        }
    }
}
