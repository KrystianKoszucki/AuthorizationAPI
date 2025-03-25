using Authorization.Tests.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Authorization.Tests.IntegrationTests
{
    public class GetUserProfile : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TokenServiceTestHelper _tokenHelper;
        public GetUserProfile(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _tokenHelper = new TokenServiceTestHelper();
        }

        [Fact]
        public async Task GetUserProfile_BannedUser_ReturnsForbidden()
        {
            var bannedUserId = 11;
            var bannedTokenUser = _tokenHelper.GenerateJwtToken(bannedUserId, "banned@banned.com", "Banned");

            var response = await GetUserProfileRequest(bannedTokenUser);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetUserProfile_ReturnsOkResult_ForValidUser()
        {
            var email = "test@test.com";
            var token = GenerateJwtToken(1, email, "3");

            var response = await GetUserProfileRequest(token);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("id", content);
            Assert.Contains("name", content);
            Assert.Contains("dateOfBirth", content);
        }

        public async Task<HttpResponseMessage> GetUserProfileRequest(string token = "")
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return await _client.GetAsync($"/authorizationapi/users/profile");
        }

        private string GenerateJwtToken(int userId, string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("completelySecureKeyHeHeINeedMoreCharactersHere");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, email),
                new(JwtRegisteredClaimNames.Email, email),
                new("id", userId.ToString()),
                new("role", role)
                };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = "",
                Issuer = ""
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
