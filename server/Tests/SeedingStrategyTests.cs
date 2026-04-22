using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using TournamentPlatformSystemWebApi.Core.Logic;

namespace Tests
{
    public class SeedingStrategyTests
    {
        [Fact]
        public void RatingSeedingStrategy_OrdersDescendingByRating()
        {
            // Arrange
            var participants = new List<BracketParticipant>
            {
                new() { Id = Guid.NewGuid(), Name = "A", Rating = 100 },
                new() { Id = Guid.NewGuid(), Name = "B", Rating = 300 },
                new() { Id = Guid.NewGuid(), Name = "C", Rating = 200 }
            };

            var strategy = new RatingSeedingStrategy();

            // Act
            var seeded = strategy.Seed(participants).ToList();

            // Assert: descending by Rating -> B (300), C (200), A (100)
            Assert.Equal(new[] { "B", "C", "A" }, seeded.Select(p => p.Name).ToArray());
        }

        [Fact]
        public void RatingSeedingStrategy_NullParticipants_ReturnsEmptyList()
        {
            var strategy = new RatingSeedingStrategy();
            var result = strategy.Seed(null!);
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
