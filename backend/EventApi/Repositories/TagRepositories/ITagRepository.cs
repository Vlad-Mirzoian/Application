using EventApi.Models;

namespace EventApi.Repositories.TagRepositories
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetByIdsAsync(List<Guid> ids);
        Task<List<Tag>> GetAllAsync();
    }
}