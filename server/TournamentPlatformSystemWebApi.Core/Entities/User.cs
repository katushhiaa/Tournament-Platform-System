using System;
using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public bool? IsOrganizer { get; set; }
        public bool IsActive { get; set; }
        public string? AccountStateDescription { get; set; }
        public UserDetail? UserDetail { get; set; }

    }
}
