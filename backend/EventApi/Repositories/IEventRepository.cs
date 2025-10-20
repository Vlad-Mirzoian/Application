using EventApi.Models;

namespace EventApi.Repositories
{
    public interface IEventRepository
    {
        Task<List<Event>> GetPublicEventsAsync();
        Task<Event?> GetByIdAsync(Guid id);
        Task AddAsync(Event @event);
        Task UpdateAsync(Event @event);
        Task DeleteAsync(Event @event);
        Task AddParticipantAsync(Guid id, Guid userId);
        Task RemoveParticipantAsync(Guid id, Guid userId);

    }
}