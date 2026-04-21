using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Core.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public bool? IsOrganizer { get; set; }
        public bool IsActive { get; set; }
        public string? AccountStateDescription { get; set; }
        public UserDetail? UserDetail { get; set; }

        public string? Password { get; set; } // not nul only in creating

    }
}
