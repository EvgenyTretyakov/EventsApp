using System.Collections.Concurrent;

using EventsWebApplication.Enums;
using EventsWebApplication.Models;
using EventsWebApplication.Services.Interfaces;

namespace EventsWebApplication.Services
{
    public class BookingQueue : IBookingQueue
    {
        private readonly ConcurrentQueue<Booking> _queue;
        private readonly ConcurrentQueue<Guid> _queueUnprocessed;

        public BookingQueue()
        {
            _queue = new ConcurrentQueue<Booking>();
            _queueUnprocessed = new ConcurrentQueue<Guid>();
        }

        public void Enqueue(Booking booking)
        {
            _queue.Enqueue(booking);
            _queueUnprocessed.Enqueue(booking.Id);
        }

        public bool TryDequeue(out Guid bookingId)
        {
            return _queueUnprocessed.TryDequeue(out bookingId);
        }

        public Booking GetBooking(Guid id)
        {
            var booking = _queue.FirstOrDefault(x => x.Id == id);

            return booking != null ? booking : null;
        }

        public bool AnyToProcess()
        {
            return _queueUnprocessed.Any();
        }

        public void UpdateStatus(Guid id)
        {
            var booking = _queue.FirstOrDefault(x => x.Id == id);

            booking?.Status = BookingStatusEnum.Confirmed;
            booking?.ProcessedAt = DateTime.Now;
        }
    }
}
