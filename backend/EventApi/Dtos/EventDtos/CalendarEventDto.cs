namespace EventApi.Dtos.EventDtos
{
    public class CalendarEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public bool IsCreator { get; set; }
    }
}