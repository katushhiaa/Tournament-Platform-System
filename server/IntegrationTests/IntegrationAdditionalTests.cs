using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class IntegrationAdditionalTests
    {
        private readonly HttpClient _client;

        public IntegrationAdditionalTests()
        {
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:5050") };
        }

        private async Task<(Guid userId, string accessToken, string refreshToken)> RegisterAndGetTokensAsync(string role)
        {
            var email = $"int_{role}_{Guid.NewGuid():N}@example.com";
            var password = "Password1";

            var register = new
            {
                Email = email,
                Password = password,
                FullName = $"Integration Tester {Guid.NewGuid():N}",
                PhoneNumber = "+380123456799",
                DateOfBirth = "1990-01-01",
                Role = role
            };

            var regResp = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
            regResp.EnsureSuccessStatusCode();

            var body = await regResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var access = doc.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString();

            // The API sets the refresh token in an HttpOnly cookie named "refresh_token".
            // Try to read it from the Set-Cookie header first; fall back to JSON token if present.
            string? refresh = null;
            if (regResp.Headers.TryGetValues("Set-Cookie", out var setCookies))
            {
                foreach (var cookie in setCookies)
                {
                    var firstPart = cookie.Split(';', 2)[0];
                    if (firstPart.StartsWith("refresh_token="))
                    {
                        refresh = firstPart.Substring("refresh_token=".Length);
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(refresh) && doc.RootElement.TryGetProperty("tokens", out var tokensEl) && tokensEl.TryGetProperty("refreshToken", out var rtEl))
            {
                refresh = rtEl.GetString();
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(access!);
            var sub = token.Subject;
            var userId = Guid.Parse(sub!);

            return (userId, access!, refresh!);
        }

        [Fact]
        public async Task Auth_Refresh_ReturnsNewAccessToken()
        {
            var (userId, access, refresh) = await RegisterAndGetTokensAsync("player");

            // Use a CookieContainer so the server reads the refresh token from Request.Cookies
            var handler = new HttpClientHandler { CookieContainer = new CookieContainer() };
            using var clientWithCookies = new HttpClient(handler) { BaseAddress = _client.BaseAddress };

            // attach Authorization header
            clientWithCookies.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access);

            // add refresh_token cookie
            handler.CookieContainer.Add(clientWithCookies.BaseAddress!, new Cookie("refresh_token", refresh));

            var resp = await clientWithCookies.PostAsync("/api/v1/auth/refresh", null);
            var body = await resp.Content.ReadAsStringAsync();
            resp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.TryGetProperty("accessToken", out var _));
        }

        [Fact]
        public async Task Sport_Get_ReturnsArray()
        {
            var resp = await _client.GetAsync("/api/v1/sport");
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.ValueKind == JsonValueKind.Array);
        }

        [Fact]
        public async Task Me_RequiresAuth_And_ReturnsProfile()
        {
            // without auth
            var unauth = await _client.GetAsync("/api/v1/users/me");
            Assert.Equal(401, (int)unauth.StatusCode);

            // with auth
            var (userId, access, refresh) = await RegisterAndGetTokensAsync("player");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access);
            var authResp = await _client.GetAsync("/api/v1/users/me");
            authResp.EnsureSuccessStatusCode();
            var body = await authResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.TryGetProperty("email", out var _));

            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task AddParticipant_DuplicateAndNotFoundCases()
        {
            // create organizer and player
            var (organizerId, organizerToken, _) = await RegisterAndGetTokensAsync("organizer");
            var (playerId, playerToken, _) = await RegisterAndGetTokensAsync("player");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", organizerToken);

            var createReq = new
            {
                Title = "Participants Extra " + Guid.NewGuid().ToString("N"),
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

            // add same participant twice -> second should be 409
            var addResp1 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", new { UserId = playerId });
            Assert.Equal(201, (int)addResp1.StatusCode);
            var addResp2 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", new { UserId = playerId });
            Assert.Equal(409, (int)addResp2.StatusCode);

            // attempt to add non-existing user
            var fakeUser = Guid.NewGuid();
            var addResp3 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", new { UserId = fakeUser });
            Assert.Equal(404, (int)addResp3.StatusCode);

            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task Disqualify_AfterStart_ReturnsConflict()
        {
            var (organizerId, organizerToken, _) = await RegisterAndGetTokensAsync("organizer");
            var (player1Id, player1Token, _) = await RegisterAndGetTokensAsync("player");
            var (player2Id, player2Token, _) = await RegisterAndGetTokensAsync("player");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", organizerToken);
            var createReq = new
            {
                Title = "StartConflict " + Guid.NewGuid().ToString("N"),
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

            // add two players
            var addResp1 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", new { UserId = player1Id });
            Assert.Equal(201, (int)addResp1.StatusCode);
            var addResp2 = await _client.PostAsJsonAsync($"/api/v1/tournaments/{tournamentId}/participants", new { UserId = player2Id });
            Assert.Equal(201, (int)addResp2.StatusCode);

            // start tournament
            var startResp = await _client.PostAsync($"/api/v1/tournaments/{tournamentId}/start", null);
            startResp.EnsureSuccessStatusCode();

            // disqualify should return 409 conflict
            var delResp = await _client.DeleteAsync($"/api/v1/tournaments/{tournamentId}/participants/{player1Id}");
            Assert.Equal(409, (int)delResp.StatusCode);

            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task Users_Search_ByFullName_ReturnsRegisteredUser()
        {
            var (userId, access, refresh) = await RegisterAndGetTokensAsync("player");
            // fetch registered user's full name using /me endpoint
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access);
            var meResp = await _client.GetAsync("/api/v1/users/me");
            meResp.EnsureSuccessStatusCode();
            var meBody = await meResp.Content.ReadAsStringAsync();
            using var meDoc = JsonDocument.Parse(meBody);
            var fullName = meDoc.RootElement.GetProperty("fullName").GetString() ?? string.Empty;
            _client.DefaultRequestHeaders.Authorization = null;

            // search by the unique full name
            var resp = await _client.GetAsync($"/api/v1/users?q={Uri.EscapeDataString(fullName)}");
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.ValueKind == JsonValueKind.Array);

            var found = false;
            foreach (var el in doc.RootElement.EnumerateArray())
            {
                if (el.TryGetProperty("id", out var idEl) && idEl.GetGuid() == userId)
                {
                    found = true;
                    break;
                }
            }

            Assert.True(found, "Registered user should be present in search results by full name");
        }

        [Fact]
        public async Task Users_Search_ByEmail_ReturnsRegisteredUser()
        {
            var (userId, access, refresh) = await RegisterAndGetTokensAsync("player");

            // register helper created email with prefix int_role_<guid>@example.com
            // extract email prefix from RegisterAndGetTokensAsync by registering separately here
            var email = $"int_player_{Guid.NewGuid():N}@example.com";
            var password = "Password1";

            var register = new
            {
                Email = email,
                Password = password,
                FullName = "SearchByEmail Tester",
                PhoneNumber = "+380123456700",
                DateOfBirth = "1990-01-01",
                Role = "player"
            };

            var regResp = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
            regResp.EnsureSuccessStatusCode();

            // search by email local-part
            var query = email.Split('@')[0];
            var resp = await _client.GetAsync($"/api/v1/users?q={Uri.EscapeDataString(query)}");
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.ValueKind == JsonValueKind.Array);

            var found = false;
            foreach (var el in doc.RootElement.EnumerateArray())
            {
                if (el.TryGetProperty("fullName", out var fnEl) && fnEl.GetString() == "SearchByEmail Tester")
                {
                    found = true;
                    break;
                }
            }

            Assert.True(found, "Registered user should be present in search results by email");
        }
    }
}
