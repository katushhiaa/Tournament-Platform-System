using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Core.Entities
{
    public class Team : BaseEntity
    {
        public string Name { get; set; } = null!;
        public virtual ICollection<UserTeam> Participants { get; set; } = new List<UserTeam>();
        public Guid TournamentId { get; set; }
        public bool? IsDisqualified { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
