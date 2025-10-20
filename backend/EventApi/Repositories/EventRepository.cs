using EventApi.Data;
using EventApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventApi.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;
        public EventRepository(AppDbContext context) => _context = context;

        public async Task<List<Event>> GetPublicEventsAsync() =>
            await _context.Events
                .Where(e => e.Visibility)
                .Include(e => e.Creator)
                .ToListAsync();

        public async Task<Event?> GetByIdAsync(Guid id) =>
            await _context.Events
                .Include(e => e.Creator)
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
    }
}
