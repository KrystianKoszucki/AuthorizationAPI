using System.Net;
using Authorization.Tests.Services;

namespace Authorization.Tests.IntegrationTests
{
    public class DeleteUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TokenServiceTestHelper _tokenHelper;

        public DeleteUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _tokenHelper = new TokenServiceTestHelper();
        }

        [Fact]
        public async Task DeleteUser_SuccessfullyDeletesUser_AdminRole()
        {
            var dummyUserId = 8;

            var token = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var response = await DeleteUserRequest(dummyUserId, token);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("Resource deleted successfully", responseBody);
        }

        [Fact]
        public async Task DeleteUser_ForbiddenForUnauthorizedUser()
        {

            var dummyUserId = 8;

            var token = _tokenHelper.GenerateJwtToken(2, "User2Surname2@email.com", "User");

            var response = await DeleteUserRequest(dummyUserId, token);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_BadRequestForInvalidUserId()
        {
            var token = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var response = await DeleteUserRequest(0, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_NotFoundForNonExistentUser()
        {
            var token = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var response = await DeleteUserRequest(9999, token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ForbiddenForBannedUser()
        {
            var bannedUserId = 7;

            var token = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");

            var response = await DeleteUserRequest(bannedUserId, token);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        }

        private async Task<HttpResponseMessage> DeleteUserRequest(int userId, string token = "")
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return await _client.DeleteAsync($"/authorizationapi/users/delete/{userId}");
        }
    }
}
