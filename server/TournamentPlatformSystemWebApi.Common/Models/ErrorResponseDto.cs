namespace TournamentPlatformSystemWebApi.Common.Models
{
    public class ErrorResponseDto
    {
        public ErrorDetail Error { get; set; } = new ErrorDetail();
    }

    public class ErrorDetail
    {
        public int Code { get; set; }
        public string? Type { get; set; }
        public string? Message { get; set; }
        public List<FieldError>? FieldErrors { get; set; }
        public string? Path { get; set; }
        public string? Timestamp { get; set; }
        public string? TraceId { get; set; }
    }

    public class FieldError
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
