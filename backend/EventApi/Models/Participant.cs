using System;

namespace EventApi.Models
{
    public class Participant
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public DateTime JoinedAt { get; set; }

        public Event Event { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}