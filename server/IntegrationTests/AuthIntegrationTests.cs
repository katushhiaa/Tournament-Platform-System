using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class AuthIntegrationTests
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests()
        {
            // When running in Docker Compose the API is available at http://app:80
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:5050") };
        }

        [Fact]
        public async Task RegisterThenLogin_Succeeds()
        {
            var email = $"testuser_{Guid.NewGuid():N}@example.com";
            var password = "Password1";

            var register = new
            {
                Email = email,
                Password = password,
                FullName = "Integration Tester",
                PhoneNumber = "+380123456789",
                DateOfBirth = "1990-01-01",
                Role = "player"
            };

            var regResp = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
            Assert.Equal(201, (int)regResp.StatusCode);

            var login = new
            {
                Email = email,
                Password = password,
                RememberMe = false
            };

            var loginResp = await _client.PostAsJsonAsync("/api/v1/auth/login", login);
            var loginContent = await loginResp.Content.ReadAsStringAsync();
            Assert.Equal(200, (int)loginResp.StatusCode);
            Assert.Contains("accessToken", loginContent, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Register_SameEmail_ReturnsConflict()
        {
            var email = $"dupuser_{Guid.NewGuid():N}@example.com";
            var password = "Password1";

            var register = new
            {
                Email = email,
                Password = password,
                FullName = "Integration Tester",
                PhoneNumber = "+380123456780",
                DateOfBirth = "1990-01-01",
                Role = "player"
            };

            var first = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
            Assert.Equal(201, (int)first.StatusCode);

            var second = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
            Assert.Equal(409, (int)second.StatusCode);
        }
    }
}
