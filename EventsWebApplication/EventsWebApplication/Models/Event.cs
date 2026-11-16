using System.ComponentModel.DataAnnotations;

namespace EventsWebApplication.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime? StartAt { get; set; }
        [Required]
        public DateTime? EndAt { get; set; }
        [Required]
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }

        public bool TryReserveSeats(int count = 1)
        {
            if (AvailableSeats - count < 0)
            {
                return false;
            }

            AvailableSeats -= count;

            return true;
        }

        public void ReleaseSeats(int count = 1)
        {
            AvailableSeats += count;
        }
    }
}
