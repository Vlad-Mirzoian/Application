using EventApi.Dtos.EventDtos;
using EventApi.Models;

namespace EventApi.Repositories.EventRepositories
{
    public interface IEventRepository
    {
        IQueryable<Event> GetPublicEventsQuery();
        Task<Event?> GetByIdAsync(Guid id);
        Task AddAsync(Event @event);
        Task UpdateAsync(Event @event);
        Task DeleteAsync(Event @event);
        Task AddParticipantAsync(Guid id, Guid userId);
        Task RemoveParticipantAsync(Guid id, Guid userId);
        Task<List<Event>> GetUserEventsAsync(Guid userId);
    }
}