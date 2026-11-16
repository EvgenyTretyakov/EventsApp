using EventsWebApplication.Enums;
using EventsWebApplication.Services.Interfaces;

namespace EventsWebApplication.Models
{
    public class Booking
    {
        private readonly IEventService _eventService;

        public Booking()
        {
        }

        public Booking(IEventService eventService)
        {
            _eventService = eventService;
        }

        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public BookingStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }

        public void Confirm()
        {
            Status = BookingStatusEnum.Confirmed;
            ProcessedAt = DateTime.Now;
        }

        public void Reject()
        {
            Status = BookingStatusEnum.Rejected;
            ProcessedAt = DateTime.Now;

            var evnt = _eventService?.GetEvent(EventId);
            evnt?.ReleaseSeats();
        }
    }
}
