using System.Collections.Generic;

using EventsWebApplication.Models;


namespace EventsWebApplication.Services.Interfaces
{
    public interface IEventService
    {
        List<Event> GetAll();
        Event? GetEvent(Guid id);
        void Add(Event newEvnt);
        void Update(Guid id, Event evnt);
        bool Delete(Guid id);
    }
}
