using System;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Core.Entities
{
    public class UserDetail : BaseEntity
    {
        public string Email { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<string> Phones { get; set; } = new List<string>();
    }
}
