using EventApi.Dtos.TagDtos;
using EventApi.Models;

namespace EventApi.Services.TagServices
{
    public interface ITagService
    {
        Task<List<Tag>> GetTagsByIdsAsync(List<Guid> tagIds);
        Task<List<TagDto>> GetAllTagsAsync();
    }
}