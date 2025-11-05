using EventApi.Dtos.TagDtos;
using EventApi.Models;

namespace EventApi.Services.TagServices
{
    public interface ITagService
    {
        Task<List<TagDto>> GetAllTagsAsync();
    }
}