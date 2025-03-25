using Authorization.Tests.Services;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Authorization.Tests.IntegrationTests
{
    public class UpdateUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TokenServiceTestHelper _tokenHelper;

        public UpdateUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _tokenHelper = new TokenServiceTestHelper();
        }

        [Fact]
        public async Task UpdateUserRole_WithoutToken_ReturnsUnauthorized()
        {
            var updateRoleBody = new UpdateRole { UserId = 1, Role = UserRoles.Admin };

            var response = await UpdateUserRoleRequest(updateRoleBody);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_NonAdmin_ReturnsForbidden()
        {
            var updateRoleBody = new UpdateRole { UserId = 5, Role = UserRoles.Admin };
            var token = _tokenHelper.GenerateJwtToken(2, "User2Surname2@email.com", "User");

            var response = await UpdateUserRoleRequest(updateRoleBody, token);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_UserNotFound_ReturnsNotFound()
        {
            var updateRoleBody = new UpdateRole { UserId = 999, Role = UserRoles.Admin };
            var token = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");

            var response = await UpdateUserRoleRequest(updateRoleBody, token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_BannedUser_ReturnsBadRequest()
        {
            var updateRoleBody = new UpdateRole { UserId = 3, Role = UserRoles.SuperAdmin };
            var token = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");

            var response = await UpdateUserRoleRequest(updateRoleBody, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_InvalidRole_ReturnsBadRequest()
        {
            var updateRoleBody = new UpdateRole { UserId = 2 };
            var token = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");

            var response = await UpdateUserRoleRequest(updateRoleBody, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_SetBannedRole_ReturnsBadRequest()
        {
            var updateRoleBody = new UpdateRole { UserId = 2, Role = UserRoles.Banned };
            var token = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");

            var response = await UpdateUserRoleRequest(updateRoleBody, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_SameRole_ReturnsBadRequest()
        {
            var updateRoleBody = new UpdateRole { UserId = 2, Role = UserRoles.User };
            var token = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");

            var response = await UpdateUserRoleRequest(updateRoleBody, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_ValidRequest_ReturnsNoContent()
        {
            var updateRoleBody = new UpdateRole { UserId = 6, Role = UserRoles.Admin };
            var token = _tokenHelper.GenerateJwtToken(1, "User1Surname1@email.com", "Admin");

            var response = await UpdateUserRoleRequest(updateRoleBody, token);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var revertRoleUpdateBody = new UpdateRole { UserId = 2, Role = UserRoles.User };
            await UpdateUserRoleRequest(revertRoleUpdateBody, token);
        }

        private async Task<HttpResponseMessage> UpdateUserRoleRequest(UpdateRole updateRoleBody, string token = "")
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
            var json = JsonSerializer.Serialize(updateRoleBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return await _client.PatchAsync("/authorizationapi/users/changeRole", content);
        }
    }
}
