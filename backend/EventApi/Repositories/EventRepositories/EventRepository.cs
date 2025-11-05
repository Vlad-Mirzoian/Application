using EventApi.Data;
using EventApi.Middlewares;
using EventApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventApi.Repositories.EventRepositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;
        public EventRepository(AppDbContext context) => _context = context;

        public IQueryable<Event> GetPublicEventsQuery()
        {
            return _context.Events
                .Where(e => e.Visibility && e.StartDateTime >= DateTime.UtcNow)
                .Include(e => e.Creator)
                .Include(e => e.Participants)
                .Include(e => e.EventTags)
                    .ThenInclude(et => et.Tag);
        }

        public async Task<Event?> GetByIdAsync(Guid id) =>
            await _context.Events
                .Include(e => e.Creator)
                .Include(e => e.Participants)
                .Include(e => e.EventTags)
                    .ThenInclude(et => et.Tag)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task AddAsync(Event @event)
        {
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event @event)
        {
            _context.Events.Update(@event);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Event @event)
        {
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }

        public async Task AddParticipantAsync(Guid eventId, Guid userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var @event = await _context.Events
                    .Include(e => e.Participants)
                    .SingleOrDefaultAsync(e => e.Id == eventId);

                if (@event == null)
                    throw new EventNotFoundException("Event not found");

                if (@event.Capacity.HasValue && @event.Participants.Count >= @event.Capacity.Value)
                    throw new EventFullException("Event is full");

                var participant = new Participant
                {
                    EventId = eventId,
                    UserId = userId
                };

                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveParticipantAsync(Guid eventId, Guid userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var participant = await _context.Participants
                    .SingleOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);

                if (participant == null)
                    throw new NotParticipantException("User is not a participant");

                _context.Participants.Remove(participant);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Event>> GetUserEventsAsync(Guid userId)
        {
            return await _context.Events
                .Where(e => e.CreatorId == userId || e.Participants.Any(p => p.UserId == userId))
                .Include(e => e.Creator)
                .Include(e => e.Participants)
                    .ThenInclude(p => p.User)
                .Include(e => e.EventTags).ThenInclude(et => et.Tag)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }
    }
}
