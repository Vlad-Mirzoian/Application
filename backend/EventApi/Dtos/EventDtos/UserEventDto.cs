namespace EventApi.Dtos.EventDtos
{
    public class UserEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public List<string> ParticipantNames { get; set; } = new();
    }
}
