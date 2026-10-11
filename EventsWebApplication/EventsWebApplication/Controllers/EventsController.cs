using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using EventsWebApplication.Models;
using EventsWebApplication.Services.Interfaces;


namespace EventsWebApplication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Метод возвращает список событий
        /// </summary>
        [HttpGet]
        public ActionResult<List<Event>> GetAllEvents()
        {
            return _eventService.GetAll();
        }

        /// <summary>
        /// Метод возвращает событие по идентификатору из коллекции
        /// </summary>
        /// <param name="id">Параметр идентификатора события</param>
        [HttpGet("{id:Guid}")]
        public IActionResult GetEventById(Guid id)
        {
            var evnt = _eventService.GetEvent(id);

            if (evnt != null)
            {
                return Ok(evnt);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Метод добавления события в коллекцию
        /// </summary>
        /// <param name="evnt">Параметр объект события</param>
        [HttpPost]
        public IActionResult AddEvent([FromBody] Event evnt)
        {
            try
            {
                _eventService.Add(evnt);

                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Метод изменения события
        /// </summary>
        /// <param name="id">Параметр идентификатор события</param>
        /// <param name="evnt">Параметр объект события</param>
        [HttpPut("{id:Guid}")]
        public IActionResult UpdateEvent(Guid id, [FromBody] Event evnt)
        {
            try
            {
                _eventService.Update(id, evnt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Метод удаления события по идентификатору из коллекции
        /// </summary>
        /// <param name="id">Параметр идентификатор события</param>
        [HttpDelete("{id:Guid}")]
        public IActionResult DeleteEvnt(Guid id)
        {
            if (_eventService.Delete(id))
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
