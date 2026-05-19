using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class TournamentsEventsIntegrationTests
    {
        private readonly HttpClient _client;

        public TournamentsEventsIntegrationTests()
        {
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:5050") };
        }

        private async Task<(Guid userId, string accessToken)> RegisterAndGetTokenAsync(string role)
        {
            var email = $"int_{role}_{Guid.NewGuid():N}@example.com";
            var password = "Password1";

            var register = new
            {
                Email = email,
                Password = password,
                FullName = "Integration Tester",
                PhoneNumber = "+380123456700",
                DateOfBirth = "1990-01-01",
                Role = role
            };

            var regResp = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
            regResp.EnsureSuccessStatusCode();

            var body = await regResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var access = doc.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString();
            Assert.False(string.IsNullOrWhiteSpace(access));

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(access!);
            var sub = token.Subject;
            var userId = Guid.Parse(sub!);

            return (userId, access!);
        }

        private void SetAuthHeader(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private void ClearAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task Events_Returns_TournamentCreated()
        {
            var (organizerId, organizerToken) = await RegisterAndGetTokenAsync("organizer");
            SetAuthHeader(organizerToken);

            var createReq = new
            {
                Title = "Events Tournament " + Guid.NewGuid().ToString("N"),
                Description = "Desc",
                Conditions = "Cond",
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(12),
                RegistrationCloseDate = DateTime.UtcNow.AddDays(9),
                MaxParticipants = 8,
                Sport = "7bf8042d-8e1a-4ffa-8ec7-baa73b86dc90"
            };

            var createResp = await _client.PostAsJsonAsync("/api/v1/tournaments", createReq);
            createResp.EnsureSuccessStatusCode();
            var createdBody = await createResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(createdBody);
            var tournamentId = doc.RootElement.GetProperty("id").GetGuid();

            // get events
            var eventsResp = await _client.GetAsync($"/api/v1/tournaments/{tournamentId}/events");
            eventsResp.EnsureSuccessStatusCode();
            var eventsBody = await eventsResp.Content.ReadAsStringAsync();
            using var edoc = JsonDocument.Parse(eventsBody);
            Assert.True(edoc.RootElement.ValueKind == JsonValueKind.Array);

            var foundCreated = false;
            foreach (var el in edoc.RootElement.EnumerateArray())
            {
                if (el.TryGetProperty("type", out var p) && p.GetString() == "tournament_created")
                {
                    foundCreated = true;
                    break;
                }
            }

            Assert.True(foundCreated, "tournament_created event not found");

            ClearAuthHeader();
        }

        [Fact]
        public async Task Events_Includes_RegistrationClosed_When_Deadline_Passed()
        {
            var (organizerId, organizerToken) = await RegisterAndGetTokenAsync("organizer");
            SetAuthHeader(organizerToken);

            var createReq = new
            {
                Title = "Events Closed Tournament " + Guid.NewGuid().ToString("N"),
                Description = "Desc",
                Conditions = "Cond",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                RegistrationCloseDate = DateTime.UtcNow.AddDays(-1), // past
                MaxParticipants = 8,
                Sport = "7bf8042d-8e1a-4ffa-8ec7-baa73b86dc90"
            };

            var createResp = await _client.PostAsJsonAsync("/api/v1/tournaments", createReq);
            createResp.EnsureSuccessStatusCode();
            var createdBody = await createResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(createdBody);
            var tournamentId = doc.RootElement.GetProperty("id").GetGuid();

            var eventsResp = await _client.GetAsync($"/api/v1/tournaments/{tournamentId}/events");
            eventsResp.EnsureSuccessStatusCode();
            var eventsBody = await eventsResp.Content.ReadAsStringAsync();
            using var edoc = JsonDocument.Parse(eventsBody);

            var found = false;
            foreach (var el in edoc.RootElement.EnumerateArray())
            {
                if (el.TryGetProperty("type", out var p) && p.GetString() == "registration_closed")
                {
                    found = true;
                    break;
                }
            }

            Assert.True(found, "registration_closed event not present");

            ClearAuthHeader();
        }

        [Fact]
        public async Task Events_Includes_TournamentStarted_After_Start()
        {
            var (organizerId, organizerToken) = await RegisterAndGetTokenAsync("organizer");
            var (player1Id, player1Token) = await RegisterAndGetTokenAsync("player");
            var (player2Id, player2Token) = await RegisterAndGetTokenAsync("player");

            SetAuthHeader(organizerToken);
            var createReq = new
            {
                Title = "Events Start Tournament " + Guid.NewGuid().ToString("N"),
                Description = "Desc",
                Conditions = "Cond",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                RegistrationCloseDate = DateTime.UtcNow.AddHours(1),
                MaxParticipants = 8,
                Sport = "7bf8042d-8e1a-4ffa-8ec7-baa73b86dc90"
            };

            var createResp = await _client.PostAsJsonAsync("/api/v1/tournaments", createReq);
            createResp.EnsureSuccessStatusCode();
            var createdBody = await createResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(createdBody);
            var tournamentId = doc.RootElement.GetProperty("id").GetGuid();

            // add two participants
            var addResp1 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", new { UserId = player1Id });
            Assert.Equal(201, (int)addResp1.StatusCode);
            var addResp2 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", new { UserId = player2Id });
            Assert.Equal(201, (int)addResp2.StatusCode);

            // start tournament
            var startResp = await _client.PostAsync($"/api/v1/tournaments/{tournamentId}/start", null);
            Assert.Equal(200, (int)startResp.StatusCode);

            var eventsResp = await _client.GetAsync($"/api/v1/tournaments/{tournamentId}/events");
            eventsResp.EnsureSuccessStatusCode();
            var eventsBody = await eventsResp.Content.ReadAsStringAsync();
            using var edoc = JsonDocument.Parse(eventsBody);

            var found = false;
            foreach (var el in edoc.RootElement.EnumerateArray())
            {
                if (el.TryGetProperty("type", out var p) && p.GetString() == "tournament_started")
                {
                    found = true;
                    break;
                }
            }

            Assert.True(found, "tournament_started event not present");

            ClearAuthHeader();
        }
    }
}
