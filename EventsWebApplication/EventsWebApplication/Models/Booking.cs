using EventsWebApplication.Enums;

namespace EventsWebApplication.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public BookingStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
