using System.ComponentModel.DataAnnotations;

namespace EventsWebApplication.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Значение Title обязательно для заполнения")]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime StartAt { get; set; }
        [Required]
        public DateTime EndAt { get; set; }
    }
}
