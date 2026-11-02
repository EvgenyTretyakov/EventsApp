using System.Collections.Concurrent;

using EventsWebApplication.Enums;
using EventsWebApplication.Models;
using EventsWebApplication.Services.Interfaces;

namespace EventsWebApplication.Services
{
    public class BookingQueue : IBookingQueue
    {
        private readonly ConcurrentQueue<Booking> _queue;

        public BookingQueue()
        {
            _queue = new ConcurrentQueue<Booking>();
        }

        public void Enqueue(Booking booking)
        {
            _queue.Enqueue(booking);
        }

        public bool TryDequeue(out Booking booking)
        {
            return _queue.TryDequeue(out booking);
        }

        public Booking GetBooking(Guid id)
        {
            var booking = _queue.FirstOrDefault(x => x.Id == id);

            return booking != null ? booking : null;
        }

        public bool AnyToProcess()
        {
            return _queue.Any(x => x.Status == BookingStatusEnum.Pending);
        }
    }
}
