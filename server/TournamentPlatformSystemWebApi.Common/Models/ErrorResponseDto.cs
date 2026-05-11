namespace TournamentPlatformSystemWebApi.Common.Models
{
    public class ErrorResponseDto
    {
        public ErrorDetail Error { get; set; } = new ErrorDetail();
        public IDictionary<string, string[]>? Errors { get; set; }
    }

    public class ErrorDetail
    {
        public int Code { get; set; }
        public string? Type { get; set; }
        public string? Message { get; set; }
        public string? Path { get; set; }
        public string? Timestamp { get; set; }
        public string? TraceId { get; set; }
    }
}
