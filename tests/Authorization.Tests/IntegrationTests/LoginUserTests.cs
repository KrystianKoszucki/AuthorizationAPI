using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Authorization.Tests.IntegrationTests
{
    public class LoginUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public LoginUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task LoginUser_ShouldReturnJwtToken()
        {
            var loginRequest = new LoginModel
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/authorizationapi/users/login", content);

            response.EnsureSuccessStatusCode();
            var responseToken = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseToken));
            var tokenParts = responseToken.Split('.');
            Assert.Equal(3, tokenParts.Length);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(responseToken);

            Assert.Equal("test@test.com", jwtToken.Claims.First(c => c.Type == "email").Value);
            Assert.Equal("User", jwtToken.Claims.First(c => c.Type == "role").Value);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnInfoAboutBan()
        {
            var loginRequest = new LoginModel
            {
                Email = "banned@banned.com",
                Password = "banned123"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/authorizationapi/users/login", content);
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status403Forbidden, (int)response.StatusCode);
            Assert.True(responseString.StartsWith("Your"), "Response message does not start with 'Your'.");
        }
    }
}
