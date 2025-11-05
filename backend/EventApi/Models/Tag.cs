namespace EventApi.Models
{
    public class Tag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();
    }
}