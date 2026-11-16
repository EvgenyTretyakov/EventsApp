using System.ComponentModel.DataAnnotations;

namespace EventsWebApplication.Models
{
    public class CreateEventDTO
    {
        [Required(ErrorMessage = "Значение Title обязательно для заполнения")]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime? StartAt { get; set; }
        [Required]
        public DateTime? EndAt { get; set; }
        [Required]
        public int TotalSeats { get; set; }
    }
}
