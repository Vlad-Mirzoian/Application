using EventApi.Dtos.TagDtos;
using EventApi.Models;
using EventApi.Repositories.TagRepositories;

namespace EventApi.Services.TagServices
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<Tag>> GetTagsByIdsAsync(List<Guid> tagIds)
        {
            if (tagIds == null || !tagIds.Any())
                return new List<Tag>();

            return await _tagRepository.GetByIdsAsync(tagIds);
        }

        public async Task<List<TagDto>> GetAllTagsAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }
    }
}