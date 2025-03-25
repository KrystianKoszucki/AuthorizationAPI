using Authorization.Tests.Services;
using System.Net;

namespace Authorization.Tests.IntegrationTests
{
    public class UndoPermabanTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TokenServiceTestHelper _tokenHelper;
        public UndoPermabanTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _tokenHelper = new TokenServiceTestHelper();
        }

        [Fact]
        public async Task UndoPermaban_Success_ReturnsNoContent()
        {
            var superadminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "SuperAdmin");
            var permabannedUserId = 12;

            var response = await UndoPermabanUserRequest(permabannedUserId, superadminToken);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UndoPermaban_UserDoesNotExist_ReturnsNotFound()
        {
            var superadminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "SuperAdmin");
            var notExistingUserId = 99999;

            var response = await UndoPermabanUserRequest(notExistingUserId, superadminToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UndoPermaban_UserNotBanned_ReturnsBadRequest()
        {
            var superadminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "SuperAdmin");
            var notExistingUserId = 3;

            var response = await UndoPermabanUserRequest(notExistingUserId, superadminToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UndoPermaban_UserNotSuperAdmin_ReturnsForbidden()
        {
            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var permabannedUserId = 11;

            var response = await UndoPermabanUserRequest(permabannedUserId, adminToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        private async Task<HttpResponseMessage> UndoPermabanUserRequest(int userId, string token = "")
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return await _client.PatchAsync($"/authorizationapi/users/undoPermaban/{userId}", null);
        }
    }
}
