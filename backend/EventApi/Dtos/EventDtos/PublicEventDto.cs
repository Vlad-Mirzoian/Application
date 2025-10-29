namespace EventApi.Dtos.EventDtos
{
    public class PublicEventDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Location { get; set; } = string.Empty;
        public List<string> ParticipantNames { get; set; } = new();
    }
}
