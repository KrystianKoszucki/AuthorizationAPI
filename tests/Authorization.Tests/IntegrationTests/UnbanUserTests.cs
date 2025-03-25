using System.Net;
using Authorization.Tests.Services;

namespace Authorization.Tests.IntegrationTests
{
    public class UnbanUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TokenServiceTestHelper _tokenHelper;
        public UnbanUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _tokenHelper = new TokenServiceTestHelper();
        }

        [Fact]
        public async Task UnbanUser_AdminUnbansBannedUser_ReturnsNoContent()
        {
            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var alreadyBannedUserId = 9;

            var response = await UnbanUserRequest(alreadyBannedUserId, adminToken);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UnbanUser_UserNotFound_ReturnsNotFound()
        {
            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var notExistingUserId = 9999;

            var response = await UnbanUserRequest(notExistingUserId, adminToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UnbanUser_PermanentlyBannedUser_ReturnsForbidden()
        {
            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var notExistingUserId = 10;

            var response = await UnbanUserRequest(notExistingUserId, adminToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UnbanUser_UserNotBanned_ReturnsBadRequest()
        {
            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var userId = 5;

            var response = await UnbanUserRequest(userId, adminToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UnbanUser_UnauthorizedUser_ReturnsUnauthorized()
        {
            var userId = 9;

            var response = await UnbanUserRequest(userId);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<HttpResponseMessage> UnbanUserRequest(int userId, string token = "")
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return await _client.PatchAsync($"/authorizationapi/users/unban/{userId}", null);
        }
    }
}
