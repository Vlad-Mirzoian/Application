using EventApi.Data;
using EventApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventApi.Repositories.TagRepositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.Tags
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
        
        public async Task<List<Guid>> GetTagsByNamesAsync(List<string> names)
        {
            return await _context.Tags
                .Where(t => names.Contains(t.Name.ToLower()))
                .Select(t => t.Id)
                .ToListAsync();
        }
    }
}