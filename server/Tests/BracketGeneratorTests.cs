using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using TournamentPlatformSystemWebApi.Core.Logic;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace Tests
{
    public class BracketGeneratorTests
    {
        [Theory]
        [InlineData(4)]
        [InlineData(7)]
        [InlineData(10)]
        public void SingleElimination_GeneratesFullBracket_CorrectMatchCounts(int participantCount)
        {
            // Arrange
            var participants = Enumerable.Range(1, participantCount)
                .Select(i => new BracketParticipant { Id = Guid.NewGuid(), Name = $"P{i}" })
                .ToList();

            var generator = new SingleEliminationBracketGenerator();
            var seeding = new RandomSeedingStrategy();

            // Act
            var matches = generator.Generate(Guid.NewGuid(), participants, seeding).ToList();

            // Compute expected bracket size and matches
            int bracketSize = 1;
            while (bracketSize < participantCount) bracketSize <<= 1;
            int expectedTotalMatches = bracketSize - 1;

            // Assert total matches
            Assert.Equal(expectedTotalMatches, matches.Count);

            // Assert rounds distribution: level 1 has bracketSize/2 matches, next halves until 1
            int expectedLevel = 1;
            int expectedMatchesThisLevel = bracketSize / 2;
            while (expectedMatchesThisLevel >= 1)
            {
                var actual = matches.Count(m => m.Level == expectedLevel);
                Assert.Equal(expectedMatchesThisLevel, actual);
                expectedLevel++;
                expectedMatchesThisLevel /= 2;
            }
        }

        [Fact]
        public void SingleElimination_EmptyParticipants_ReturnsNoMatches()
        {
            var participants = new List<BracketParticipant>();
            var generator = new SingleEliminationBracketGenerator();
            var seeding = new RandomSeedingStrategy();

            var matches = generator.Generate(Guid.NewGuid(), participants, seeding);

            Assert.Empty(matches);
        }

        [Fact]
        public void SingleElimination_NullParticipants_Throws()
        {
            var generator = new SingleEliminationBracketGenerator();
            var seeding = new RandomSeedingStrategy();

            Assert.Throws<ArgumentNullException>(() => generator.Generate(Guid.NewGuid(), null!, seeding));
        }

        [Fact]
        public void SingleElimination_NullSeedingStrategy_Throws()
        {
            var participants = Enumerable.Range(1, 4)
                .Select(i => new BracketParticipant { Id = Guid.NewGuid(), Name = $"P{i}" })
                .ToList();

            var generator = new SingleEliminationBracketGenerator();

            Assert.Throws<ArgumentNullException>(() => generator.Generate(Guid.NewGuid(), participants, null!));
        }
    }
}
