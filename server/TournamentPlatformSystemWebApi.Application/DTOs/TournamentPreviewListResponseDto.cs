using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class TournamentPreviewListResponseDto
    {
        [JsonPropertyName("tournaments")]
        public IReadOnlyList<TournamentPreviewDto> Tournaments { get; set; } = Array.Empty<TournamentPreviewDto>();

        [JsonPropertyName("is_personalized")]
        public bool IsPersonalized { get; set; }

        [JsonPropertyName("fallback_reason")]
        public string? FallbackReason { get; set; }
    }
}
