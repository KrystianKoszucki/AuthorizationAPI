using Authorization.Tests.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Authorization.Tests.IntegrationTests
{
    public class ChangePasswordTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TokenServiceTestHelper _tokenHelper;

        public ChangePasswordTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _tokenHelper = new TokenServiceTestHelper();
        }

        [Fact]
        public async Task UpdateUserPassword_ReturnsNoContent_ForValidPasswordChange()
        {
            var jwtToken = _tokenHelper.GenerateJwtToken(1, "User1Surname1@email.com", "3");
            var response = await SendPasswordChangeRequest(jwtToken, "testUser1Surname1", "NewSecurePass123!");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await SendPasswordChangeRequest(jwtToken, "NewSecurePass123!", "testUser1Surname1");
        }

        [Fact]
        public async Task UpdateUserPassword_ReturnsBadRequest_WhenOldPasswordIsIncorrect()
        {
            var jwtToken = _tokenHelper.GenerateJwtToken(1, "User1Surname1@email.com", "3");
            var response = await SendPasswordChangeRequest(jwtToken, "wrongPassword", "NewSecurePass123!");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserPassword_ReturnsBadRequest_WhenNewPasswordIsEmpty()
        {
            var jwtToken = _tokenHelper.GenerateJwtToken(1, "User1Surname1@email.com", "3");
            var response = await SendPasswordChangeRequest(jwtToken, "testUser1Surname1", "");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserPassword_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var notExistingUserId = 9991;
            var jwtToken = _tokenHelper.GenerateJwtToken(notExistingUserId, "test999@test.com", "3");
            var response = await SendPasswordChangeRequest(jwtToken, "SomePassword123", "NewSecurePass123!");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserPassword_ReturnsUnauthorized_WhenNoTokenProvided()
        {
            var response1 = await SendPasswordChangeRequest("invalidToken", "testUser1Surname1", "NewSecurePass123!");

            Assert.Equal(HttpStatusCode.Unauthorized, response1.StatusCode);
        }

        [Fact]
        public async Task UpdateUserPassword_ReturnsUnauthorized_ForInvalidToken()
        {
            var response = await SendPasswordChangeRequest("invalidToken", "testUser1Surname1", "NewSecurePass123!");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<HttpResponseMessage> SendPasswordChangeRequest(string token, string oldPassword, string newPassword)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, "/authorizationapi/users/changePassword");
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(new
            {
                OldPassword = oldPassword,
                NewPassword = newPassword
            }), Encoding.UTF8, "application/json");

            return await _client.SendAsync(request);
        }
    }
}
