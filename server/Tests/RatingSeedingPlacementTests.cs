using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using TournamentPlatformSystemWebApi.Core.Logic;

namespace Tests
{
    public class RatingSeedingPlacementTests
    {
        [Fact]
        public void RatingSeeding_PairsCorrectly_For4Participants()
        {
            // Arrange: ratings descending should result in A,B,C,D
            var participants = new List<BracketParticipant>
            {
                new() { Id = Guid.NewGuid(), Name = "A", Rating = 400 },
                new() { Id = Guid.NewGuid(), Name = "B", Rating = 300 },
                new() { Id = Guid.NewGuid(), Name = "C", Rating = 200 },
                new() { Id = Guid.NewGuid(), Name = "D", Rating = 100 }
            };

            var generator = new SingleEliminationBracketGenerator();
            var seeding = new RatingSeedingStrategy();

            // Act
            var matches = generator.Generate(Guid.NewGuid(), participants, seeding).Where(m => m.Level == 1).OrderBy(m => m.OrderNumber).ToList();

            // Assert: classical seeding pairs are (A vs D) and (B vs C)
            Assert.Equal("A", matches[0].TeamAName);
            Assert.Equal("D", matches[0].TeamBName);
            Assert.Equal("B", matches[1].TeamAName);
            Assert.Equal("C", matches[1].TeamBName);
        }

        [Fact]
        public void RatingSeeding_PairsCorrectly_For7Participants()
        {
            // Arrange: names reflect rating order A..G (700..100)
            var participants = new List<BracketParticipant>
            {
                new() { Id = Guid.NewGuid(), Name = "A", Rating = 700 },
                new() { Id = Guid.NewGuid(), Name = "B", Rating = 600 },
                new() { Id = Guid.NewGuid(), Name = "C", Rating = 500 },
                new() { Id = Guid.NewGuid(), Name = "D", Rating = 400 },
                new() { Id = Guid.NewGuid(), Name = "E", Rating = 300 },
                new() { Id = Guid.NewGuid(), Name = "F", Rating = 200 },
                new() { Id = Guid.NewGuid(), Name = "G", Rating = 100 }
            };

            var generator = new SingleEliminationBracketGenerator();
            var seeding = new RatingSeedingStrategy();

            // Act
            var matches = generator.Generate(Guid.NewGuid(), participants, seeding).Where(m => m.Level == 1).OrderBy(m => m.OrderNumber).ToList();

            // Assert: classical seeding for 7 participants (8-slot bracket) results in pairs:
            // (A vs bye), (D vs E), (B vs G), (C vs F)
            Assert.Equal("A", matches[0].TeamAName);
            Assert.Null(matches[0].TeamBName);
            Assert.Equal(matches[0].TeamAId, matches[0].WinnerId);

            Assert.Equal("D", matches[1].TeamAName);
            Assert.Equal("E", matches[1].TeamBName);

            Assert.Equal("B", matches[2].TeamAName);
            Assert.Equal("G", matches[2].TeamBName);

            Assert.Equal("C", matches[3].TeamAName);
            Assert.Equal("F", matches[3].TeamBName);
        }

        [Fact]
        public void RatingSeeding_PairsCorrectly_For10Participants()
        {
            // Arrange: names reflect rating order A..J (1000..100)
            var participants = Enumerable.Range(0, 10)
                .Select(i => new BracketParticipant { Id = Guid.NewGuid(), Name = ((char)('A' + i)).ToString(), Rating = (10 - i) * 100 })
                .ToList();

            var generator = new SingleEliminationBracketGenerator();
            var seeding = new RatingSeedingStrategy();
            // a b c d e f g h i j 
            // Act
            var matches = generator.Generate(Guid.NewGuid(), participants, seeding).Where(m => m.Level == 1).OrderBy(m => m.OrderNumber).ToList();

            // For 10 participants in 16-slot bracket, ensure top seeds are distributed: no two top-4 seeds share the same level-1 match
            var top4 = new[] { "A", "B", "C", "D" };
            foreach (var m in matches)
            {
                var names = new[] { m.TeamAName, m.TeamBName };
                var intersection = names.Intersect(top4).ToList();
                Assert.True(intersection.Count <= 1, "Two top-4 seeds ended up in same first-round match");
            }
        }

        [Fact]
        public void RatingSeeding_PairsCorrectly_For9Participants()
        {
            // Arrange: names reflect rating order A..I (900..100)
            var participants = Enumerable.Range(0, 9)
                .Select(i => new BracketParticipant { Id = Guid.NewGuid(), Name = ((char)('A' + i)).ToString(), Rating = (9 - i) * 100 })
                .ToList();

            var generator = new SingleEliminationBracketGenerator();
            var seeding = new RatingSeedingStrategy();

            // Act
            var matches = generator.Generate(Guid.NewGuid(), participants, seeding).Where(m => m.Level == 1).OrderBy(m => m.OrderNumber).ToList();

            // Assert: 9 participants -> 16-slot bracket -> 8 matches in first round
            Assert.Equal(8, matches.Count);

            // With 9 participants in 16 slots there should be 7 byes (matches with exactly one participant)
            var byeCount = matches.Count(m => m.TeamAName == null || m.TeamBName == null);
            Assert.Equal(7, byeCount);

            // Top-4 seeds should be distributed across different first-round matches
            var top4 = new[] { "A", "B", "C", "D" };
            foreach (var m in matches)
            {
                var names = new[] { m.TeamAName, m.TeamBName };
                var intersection = names.Intersect(top4).ToList();
                Assert.True(intersection.Count <= 1, "Two top-4 seeds ended up in same first-round match");
            }

            // Additionally, top seeds should receive byes in this configuration (top 7 get byes)
            foreach (var top in top4)
            {
                var match = matches.First(m => m.TeamAName == top || m.TeamBName == top);
                // the opponent for a top seed in this scenario should be null (bye)
                if (match.TeamAName == top)
                    Assert.Null(match.TeamBName);
                else
                    Assert.Null(match.TeamAName);
            }
        }
    }
}
