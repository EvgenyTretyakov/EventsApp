using System.ComponentModel.DataAnnotations;

using EventsWebApplication.Models;
using EventsWebApplication.Exceptions;
using EventsWebApplication.Services.Interfaces;

namespace EventsWebApplication.Services
{
    public class EventService : IEventService
    {
        //Коллекция для манипуляции событиями
        private List<Event> _events = new();

        //Метод добавления нового события
        public void Add(Event newEvnt)
        {
            var sameEvent = _events.FirstOrDefault(x =>
                x.Title == newEvnt.Title &&
                x.StartAt == newEvnt.StartAt &&
                x.EndAt == newEvnt.EndAt);

            if (sameEvent == null)
            {
                var result = DateTime.Compare(newEvnt.StartAt.Value, newEvnt.EndAt.Value);
                if (result > 0 || result == 0)
                {
                    throw new ValidationException("Даты окончания и начала события не корректны.");
                }

                newEvnt.Id = newEvnt.Id != Guid.Empty ? newEvnt.Id : Guid.NewGuid();
                _events.Add(newEvnt);
            }
            else
            {
                throw new ValidationException("Такое событие уже есть в списке!");
            }
        }

        //Метод удаления события
        public bool Delete(Guid id)
        {
            var existEvent = _events.FirstOrDefault(x => x.Id == id);

            if (existEvent == null)
            {
                throw new NotFoundException("Событие не найдено.");
            }

            return _events.Remove(existEvent);
        }

        //Метод получения всех событий
        public PagedResult<Event> GetAll(string? title, DateTime? from, DateTime? to, int? page = 1, int? pageSize = 10)
        {
            var eventsFiltered = _events.AsEnumerable();

            if (!string.IsNullOrEmpty(title))
            {
                eventsFiltered = _events.Where(x => x.Title.ToUpper().Contains(title.ToUpper()));
            }

            if (from.HasValue)
            {
                eventsFiltered = eventsFiltered.Where(x => x.StartAt.HasValue).Where(x => x.StartAt.Value.Date >= from.Value.Date);
            }

            if (to.HasValue)
            {
                eventsFiltered = eventsFiltered.Where(x => x.EndAt.HasValue).Where(x => x.EndAt.Value.Date <= to.Value.Date);
            }

            var currentPageItems = eventsFiltered
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();

            return new PagedResult<Event>
            {
                TotalItems = eventsFiltered.Count(),
                Items = currentPageItems,
                CurrentPage = page.Value,
                CurrentPageTotalItems = currentPageItems.Count
            };
        }

        //Метод получения события по идентификатору
        public Event GetEvent(Guid id)
        {
            var eventData = _events.FirstOrDefault(x => x.Id == id);

            if (eventData == null)
            {
                throw new NotFoundException("Событие не найдено.");
            }

            return eventData;
        }

        //Метод изменения информации события
        public void Update(Guid id, Event newEvnt)
        {
            var oldEvnt = _events.FirstOrDefault(x => x.Id == id);
            if (oldEvnt == null)
            {
                throw new NotFoundException("Событие не найдено.");
            }

            if (oldEvnt.Title != newEvnt.Title ||
                oldEvnt.Description != newEvnt.Description ||
                oldEvnt.StartAt != newEvnt.StartAt ||
                oldEvnt.EndAt != newEvnt.EndAt)
            {
                var result = DateTime.Compare(newEvnt.StartAt.Value, newEvnt.EndAt.Value);
                if (result > 0)
                {
                    throw new ValidationException("Дата окончания события должна быть больше даты начала.");
                }

                oldEvnt.Title = newEvnt.Title;
                oldEvnt.Description = newEvnt.Description;
                oldEvnt.StartAt = newEvnt.StartAt;
                oldEvnt.EndAt = newEvnt.EndAt;
            }
        }
    }
}
