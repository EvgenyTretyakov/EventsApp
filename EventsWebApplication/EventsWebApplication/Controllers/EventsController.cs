using Microsoft.AspNetCore.Mvc;

using EventsWebApplication.Models;
using EventsWebApplication.Services.Interfaces;


namespace EventsWebApplication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IBookingService _bookingService;

        public EventsController(IEventService eventService, IBookingService bookingService)
        {
            _eventService = eventService;
            _bookingService = bookingService;
        }

        /// <summary>
        /// Метод возвращает DTO PaginatedResult включающий:
        /// общее количество событий, массив самих событий, номер текущей страницы, количество элементов на текущей странице
        /// </summary>
        /// <param name="title">Поиск по названию</param>
        /// <param name="from">События, которые начинаются не раньше указанной даты</param>
        /// <param name="to">События, которые заканчиваются не позже указанной даты</param>
        /// <param name="page">Страница, которую необходимо вернуть</param>
        /// <param name="pageSize">Количество элементов на странице</param>
        [HttpGet]
        public ActionResult<PagedResult<Event>> GetAllEvents(
            [FromQuery] string? title = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int? page = 1,
            [FromQuery] int? pageSize = 10)
        {
            return _eventService.GetAll(title, from, to, page, pageSize);
        }

        /// <summary>
        /// Метод возвращает событие по идентификатору из коллекции
        /// </summary>
        /// <param name="id">Параметр идентификатора события</param>
        [HttpGet("{id:Guid}")]
        public IActionResult GetEventById(Guid id)
        {
            var evnt = _eventService.GetEvent(id);

            return Ok(evnt);
        }

        /// <summary>
        /// Метод добавления события в коллекцию
        /// </summary>
        /// <param name="evnt">Параметр объект события</param>
        [HttpPost]
        public IActionResult AddEvent([FromBody] Event evnt)
        {
            _eventService.Add(evnt);

            return Created();
        }

        /// <summary>
        /// Метод изменения события
        /// </summary>
        /// <param name="id">Параметр идентификатор события</param>
        /// <param name="evnt">Параметр объект события</param>
        [HttpPut("{id:Guid}")]
        public IActionResult UpdateEvent(Guid id, [FromBody] Event evnt)
        {
            _eventService.Update(id, evnt);

            return NoContent();
        }

        /// <summary>
        /// Метод удаления события по идентификатору из коллекции
        /// </summary>
        /// <param name="id">Параметр идентификатор события</param>
        [HttpDelete("{id:Guid}")]
        public IActionResult DeleteEvnt(Guid id)
        {
            _eventService.Delete(id);

            return NoContent();
        }

        /// <summary>
        /// Метод бронирования
        /// </summary>
        /// <param name="id">Параметр идентификатор события</param>
        [HttpPost("{id:Guid}/book")]
        public async Task<IActionResult> Book(Guid id)
        {
            var evnt = _eventService.GetEvent(id);

            var booking = await _bookingService.CreateBookingAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            var result = Accepted(new { booking.Id, booking.EventId, booking.Status });
            result.Location = $"/bookongs/{booking.Id}";

            return result;
        }

        /// <summary>
        /// Метод возвращает текущее состояние брони по её идентификатору
        /// </summary>
        /// <param name="id">Параметр идентификатор брони</param>
        [HttpPost("/bookings/{id:Guid}")]
        public async Task<IActionResult> GetBookings(Guid id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(booking);
            }
        }
    }
}
