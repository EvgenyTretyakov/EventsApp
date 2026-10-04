using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using EventsWebApplication.Models;
using EventsWebApplication.Services.Interfaces;


namespace EventsWebApplication.Controllers
{
    [Route("api/[controller]")]
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
        public ApiResult<List<Event>> GetAllEvents()
        {
            return new ApiResult<List<Event>>
            {
                Data = _eventService.GetAll(),
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Message = "Получаем все события"
            };
        }

        /// <summary>
        /// Метод возвращает событие по идентификатору из коллекции
        /// </summary>
        /// <param name="id">Параметр идентификатора события</param>
        [HttpGet("{id:Guid}")]
        public ApiBaseResult GetEventById(Guid id)
        {
            var evnt = _eventService.GetEvent(id);

            if (evnt != null)
            {
                return new ApiResult<Event>
                {
                    Data = evnt,
                    Success = true,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Получаем событие по id из списка"
                };
            }
            else
            {
                return new ApiResult
                {
                    Success = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Не удалось найти событие по id"
                };
            }
        }

        /// <summary>
        /// Метод добавления события в коллекцию
        /// </summary>
        /// <param name="evnt">Параметр объект события</param>
        [HttpPost]
        public ApiResult AddEvent([FromBody] Event evnt)
        {
            try
            {
                _eventService.Add(evnt);

                return new ApiResult
                {
                    Success = true,
                    StatusCode = HttpStatusCode.Created,
                    Message = "Добавлено событие в коллекцию"
                };
            }
            catch (Exception ex)
            {
                return new ApiResult
                {
                    Success = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = $"Не удалось добавить событие. Ошибка:{ex.Message}"
                };
            }
        }

        /// <summary>
        /// Метод изменения события
        /// </summary>
        /// <param name="id">Параметр идентификатор события</param>
        /// <param name="evnt">Параметр объект события</param>
        [HttpPut("{id:Guid}")]
        public ApiResult UpdateEvent(Guid id, [FromBody] Event evnt)
        {
            var existEvnt = _eventService.GetEvent(id);

            if (existEvnt != null)
            {
                try
                {
                    _eventService.Update(id, evnt);
                }
                catch (Exception ex)
                {
                    return new ApiResult
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = $"Не удалось обновить событие. Ошибка:{ex.Message}"
                    };
                }

                return new ApiResult
                {
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                    Message = "Данные события обновлены."
                };
            }
            else
            {
                return new ApiResult
                {
                    Success = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Событие не найдено."
                };
            }
        }

        /// <summary>
        /// Метод удаления события по идентификатору из коллекции
        /// </summary>
        /// <param name="id">Параметр идентификатор события</param>
        [HttpDelete("{id:Guid}")]
        public ApiResult DeleteEvnt(Guid id)
        {
            var existEvnt = _eventService.GetEvent(id);

            if (existEvnt != null)
            {
                _eventService.Delete(id);

                return new ApiResult
                {
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                    Message = "Событие удалено."
                };
            }
            else
            {
                return new ApiResult
                {
                    Success = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Событие не найдено."
                };
            }
        }
    }
}
