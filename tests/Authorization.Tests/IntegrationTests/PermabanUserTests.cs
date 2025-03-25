using Authorization.Tests.Services;
using System.Net;

namespace Authorization.Tests.IntegrationTests
{
    public class PermabanUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TokenServiceTestHelper _tokenHelper;

        public PermabanUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _tokenHelper = new TokenServiceTestHelper();
        }

        [Fact]
        public async Task PermabanUser_Success_ReturnsNoContent()
        {
            var superadminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "SuperAdmin");
            var userId = 3;

            var response = await PermabanUserRequest(userId, superadminToken);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PermabanUser_UserDoesNotExist_ReturnsNotFound()
        {
            var superadminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "SuperAdmin");
            var notExistingUser = 99999;

            var response = await PermabanUserRequest(notExistingUser, superadminToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PermabanUser_AlreadyPermabanned_ReturnsForbidden()
        {
            var superadminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "SuperAdmin");
            var alreadyPermabannedUserId = 10;

            var response = await PermabanUserRequest(alreadyPermabannedUserId, superadminToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task PermabanUser_TryingToBanSelf_ReturnsForbidden()
        {
            var superadminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "SuperAdmin");
            var superadminId = 4;

            var response = await PermabanUserRequest(superadminId, superadminToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task PermabanUser_UserNotSuperAdmin_ReturnsForbidden()
        {
            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var userId = 2;

            var response = await PermabanUserRequest(userId, adminToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        private async Task<HttpResponseMessage> PermabanUserRequest(int userId, string token = "")
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return await _client.PatchAsync($"/authorizationapi/users/permaban/{userId}", null);
        }
    }
}
