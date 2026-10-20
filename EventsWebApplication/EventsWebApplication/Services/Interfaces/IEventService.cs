using System.Collections.Generic;

using EventsWebApplication.Models;


namespace EventsWebApplication.Services.Interfaces
{
    public interface IEventService
    {
        PagedResult<Event> GetAll(string? title, DateTime? from, DateTime? to, int? page = 1, int? pageSize = 10);
        Event? GetEvent(Guid id);
        void Add(Event newEvnt);
        void Update(Guid id, Event evnt);
        bool Delete(Guid id);
    }
}
