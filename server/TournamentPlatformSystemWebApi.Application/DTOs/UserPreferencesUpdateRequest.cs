using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TournamentPlatformSystemWebApi.Application.DTOs;

public class UserPreferencesUpdateRequest
{
    [Required]
    public IReadOnlyList<Guid> ThemeIds { get; set; } = new List<Guid>();
}
