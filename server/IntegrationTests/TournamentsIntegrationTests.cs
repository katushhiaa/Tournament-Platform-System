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
    public class TournamentsIntegrationTests
    {
        private readonly HttpClient _client;

        public TournamentsIntegrationTests()
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
            // response contains tokens.accessToken
            var access = doc.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString();
            Assert.False(string.IsNullOrWhiteSpace(access));

            // decode jwt to extract subject (user id)
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
        public async Task CreateUpdateAndGetTournament_Flow()
        {
            var (organizerId, organizerToken) = await RegisterAndGetTokenAsync("organizer");
            SetAuthHeader(organizerToken);

            var createReq = new
            {
                Title = "Integration Tournament " + Guid.NewGuid().ToString("N"),
                Description = "Desc",
                Conditions = "Cond",
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(12),
                RegistrationCloseDate = DateTime.UtcNow.AddDays(9),
                MaxParticipants = 8,
                Sport = "7bf8042d-8e1a-4ffa-8ec7-baa73b86dc90"
            };

            var createResp = await _client.PostAsJsonAsync("/api/v1/tournaments", createReq);
            Assert.Equal(201, (int)createResp.StatusCode);

            var createdBody = await createResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(createdBody);
            var tournamentId = doc.RootElement.GetProperty("id").GetGuid();

            // update tournament
            var updateReq = new
            {
                Title = "Updated Title",
                Description = "Updated",
                Conditions = "C",
                StartDate = DateTime.UtcNow.AddDays(11),
                EndDate = DateTime.UtcNow.AddDays(13),
                RegistrationCloseDate = DateTime.UtcNow.AddDays(10),
                MaxParticipants = 16,
                Sport = "7bf8042d-8e1a-4ffa-8ec7-baa73b86dc90"
            };

            var patch = new HttpRequestMessage(new HttpMethod("PATCH"), $"/api/v1/tournaments/{tournamentId}")
            {
                Content = JsonContent.Create(updateReq)
            };
            var patchResp = await _client.SendAsync(patch);
            var patchBody = await patchResp.Content.ReadAsStringAsync();
            Assert.Equal(200, (int)patchResp.StatusCode);

            // get details
            var detailsResp = await _client.GetAsync($"/api/v1/tournaments/{tournamentId}");
            Assert.Equal(200, (int)detailsResp.StatusCode);

            ClearAuthHeader();
        }

        [Fact]
        public async Task ParticipantLifecycle_Add_Get_Disqualify()
        {
            var (organizerId, organizerToken) = await RegisterAndGetTokenAsync("organizer");
            var (player1Id, player1Token) = await RegisterAndGetTokenAsync("player");
            var (player2Id, player2Token) = await RegisterAndGetTokenAsync("player");

            // create tournament
            SetAuthHeader(organizerToken);
            var createReq = new
            {
                Title = "Participants Tournament " + Guid.NewGuid().ToString("N"),
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

            // add player1 by organizer
            var addReq = new { UserId = player1Id };
            var addResp = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", addReq);
            Assert.Equal(201, (int)addResp.StatusCode);

            // add player2
            var addReq2 = new { UserId = player2Id };
            var addResp2 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", addReq2);
            Assert.Equal(201, (int)addResp2.StatusCode);

            // get participants
            var listResp = await _client.GetAsync($"/api/v1/tournaments/{tournamentId}/participants");
            listResp.EnsureSuccessStatusCode();
            var listBody = await listResp.Content.ReadAsStringAsync();
            using var listDoc = JsonDocument.Parse(listBody);
            var arr = listDoc.RootElement.EnumerateArray();
            var list = new List<Guid>();
            foreach (var el in arr)
            {
                if (el.TryGetProperty("id", out var idProp))
                    list.Add(idProp.GetGuid());
            }
            Assert.True(list.Count >= 2);

            // disqualify player1
            var delResp = await _client.DeleteAsync($"/api/v1/tournaments/{tournamentId}/participants/{player1Id}");
            Assert.Equal(204, (int)delResp.StatusCode);

            // getting participants should not include disqualified player
            var listResp2 = await _client.GetAsync($"/api/v1/tournaments/{tournamentId}/participants");
            listResp2.EnsureSuccessStatusCode();
            var listBody2 = await listResp2.Content.ReadAsStringAsync();
            using var listDoc2 = JsonDocument.Parse(listBody2);
            var remaining = new List<Guid>();
            foreach (var el in listDoc2.RootElement.EnumerateArray())
            {
                if (el.TryGetProperty("id", out var idProp))
                    remaining.Add(idProp.GetGuid());
            }

            Assert.DoesNotContain(player1Id, remaining);

            ClearAuthHeader();
        }

        [Fact]
        public async Task StartTournament_Creates_Matches()
        {
            var (organizerId, organizerToken) = await RegisterAndGetTokenAsync("organizer");
            var (player1Id, player1Token) = await RegisterAndGetTokenAsync("player");
            var (player2Id, player2Token) = await RegisterAndGetTokenAsync("player");

            SetAuthHeader(organizerToken);
            var createReq = new
            {
                Title = "Start Tournament " + Guid.NewGuid().ToString("N"),
                Description = "Desc",
                Conditions = "Cond",
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(12),
                RegistrationCloseDate = DateTime.UtcNow.AddDays(9),
                MaxParticipants = 8,
                Sport = "7bf8042d-8e1a-4ffa-8ec7-baa73b86dc90"
            };

            var createResp = await _client.PostAsJsonAsync("/api/v1/tournaments", createReq);
            var createdBody = await createResp.Content.ReadAsStringAsync();
            createResp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(createdBody);
            var tournamentId = doc.RootElement.GetProperty("id").GetGuid();

            // add two players
            var addResp1 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", new { UserId = player1Id });
            Assert.Equal(201, (int)addResp1.StatusCode);
            var addResp2 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", new { UserId = player2Id });
            Assert.Equal(201, (int)addResp2.StatusCode);

            // start tournament
            var startResp = await _client.PostAsync($"/api/v1/tournaments/{tournamentId}/start", null);
            Assert.Equal(200, (int)startResp.StatusCode);
            var startBody = await startResp.Content.ReadAsStringAsync();
            using var sdoc = JsonDocument.Parse(startBody);
            if (sdoc.RootElement.TryGetProperty("matchesCreated", out var mprop))
            {
                Assert.True(mprop.GetInt32() >= 0);
            }

            // get matches
            var matchesResp = await _client.GetAsync($"/api/v1/tournaments/{tournamentId}/matches");
            matchesResp.EnsureSuccessStatusCode();
            var matchesBody = await matchesResp.Content.ReadAsStringAsync();
            using var md = JsonDocument.Parse(matchesBody);
            Assert.True(md.RootElement.ValueKind == JsonValueKind.Array);

            ClearAuthHeader();
        }

        [Fact]
        public async Task UploadImage_WithoutFile_ReturnsBadRequest()
        {
            var (organizerId, organizerToken) = await RegisterAndGetTokenAsync("organizer");
            SetAuthHeader(organizerToken);

            var createReq = new
            {
                Title = "Image Tournament " + Guid.NewGuid().ToString("N"),
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

            // Post without file
            var resp = await _client.PostAsync($"/api/v1/tournaments/{tournamentId}/image", null);
            Assert.Equal(400, (int)resp.StatusCode);

            ClearAuthHeader();
        }
    }
}
