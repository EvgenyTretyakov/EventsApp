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

        public bool TryDequeue(out Guid bookingId)
        {
            var result = _queue.TryDequeue(out var booking);
            bookingId = booking != null ? booking.Id : Guid.Empty;

            return result;
        }

        public Booking GetBooking(Guid id)
        {
            var booking = _queue.FirstOrDefault(x => x.Id == id);

            return booking != null ? booking : null;
        }

        public void UpdateStatus(Guid id)
        {
            var booking = _queue.FirstOrDefault(x => x.Id == id);

            if (booking == null)
            {
                throw new Exception("Бронь не найдена!");
            }

            booking.Status = BookingStatusEnum.Confirmed;
            booking.ProcessedAt = DateTime.Now;
        }

        public IEnumerable<Booking> GetPending()
        {
            return _queue.Where(x => x.Status == BookingStatusEnum.Pending);
        }

        public void Update(Booking booking)
        {
            var bookingOld = _queue.FirstOrDefault(x => x.Id == booking.Id);

            if (bookingOld == null)
            {
                throw new Exception("Бронь не найдена!");
            }

            bookingOld.Status = booking.Status;
            bookingOld.ProcessedAt = DateTime.Now;
        }
    }
}
