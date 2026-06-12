using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
