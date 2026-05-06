using System;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Core.Entities;

public class TournamentTheme : BaseEntity
{
    public string Name { get; set; } = null!;
}
