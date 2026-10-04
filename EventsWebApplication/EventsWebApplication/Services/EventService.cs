using EventsWebApplication.Models;
using EventsWebApplication.Services.Interfaces;

namespace EventsWebApplication.Services
{
    public class EventService : IEventService
    {
        //Коллекция для манипуляции событиями
        private static List<Event> _events = new();

        //Метод добавления нового события
        public void Add(Event newEvnt)
        {
            var sameEvent = _events.FirstOrDefault(x =>
                x.Title == newEvnt.Title &&
                x.StartAt == newEvnt.StartAt &&
                x.EndAt == newEvnt.EndAt);

            if (sameEvent == null)
            {
                var result = DateTime.Compare(newEvnt.StartAt, newEvnt.EndAt);
                if (result > 0)
                {
                    throw new Exception("Дата окончания события должна быть больше даты начала.");
                }

                newEvnt.Id = Guid.NewGuid();
                _events.Add(newEvnt);
            }
            else
            {
                throw new Exception("Такое событие уже есть в списке!");
            }
        }

        //Метод удаления события
        public void Delete(Guid id)
        {
            var existEvent = _events.FirstOrDefault(x => x.Id == id);

            if (existEvent != null)
            {
                _events.Remove(existEvent);
            }
        }

        //Метод получения всех событий
        public List<Event> GetAll()
        {
            return _events;
        }

        //Метод получения события по идентификатору
        public Event? GetEvent(Guid id)
        {
            return _events.FirstOrDefault(x => x.Id == id);
        }

        //Метод изменения информации события
        public void Update(Guid id, Event newEvnt)
        {
            var oldEvnt = _events.FirstOrDefault(x => x.Id == id);

            if (oldEvnt != null &&
                (oldEvnt.Title != newEvnt.Title ||
                 oldEvnt.Description != newEvnt.Description ||
                 oldEvnt.StartAt != newEvnt.StartAt ||
                 oldEvnt.EndAt != newEvnt.EndAt))
            {
                var result = DateTime.Compare(newEvnt.StartAt, newEvnt.EndAt);
                if (result > 0)
                {
                    throw new Exception("Дата окончания события должна быть больше даты начала.");
                }

                oldEvnt.Title = newEvnt.Title;
                oldEvnt.Description = newEvnt.Description;
                oldEvnt.StartAt = newEvnt.StartAt;
                oldEvnt.EndAt = newEvnt.EndAt;
            }
        }
    }
}
