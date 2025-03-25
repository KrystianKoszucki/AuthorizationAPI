using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Authorization.Tests.Services;

namespace Authorization.Tests.IntegrationTests
{
    public class BanUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TokenServiceTestHelper _tokenHelper;

        public BanUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _tokenHelper = new TokenServiceTestHelper();
        }

        [Fact]
        public async Task BanUser_WhenUserExists_ShouldReturnNoContent()
        {
            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");

            var response = await BanUserRequest(2, adminToken);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task BanUser_WhenUserDoesNotExist_ShouldReturnNotFound()
        {
            var nonExistentUserId = 9999;

            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");

            var response = await BanUserRequest(nonExistentUserId, adminToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task BanUser_WhenUserIsAlreadyBanned_ShouldReturnBadRequest()
        {
            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");
            var alreadyBannedUserId = 7;

            var response = await BanUserRequest(alreadyBannedUserId, adminToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task BanUser_WhenUserTriesToBanThemself_ShouldReturnForbidden()
        {
            var adminId = 4;
            var adminToken = _tokenHelper.GenerateJwtToken(adminId, "User4Surname4@email.com", "Admin");

            var response = await BanUserRequest(adminId, adminToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task BanUser_WhenAdminTriesToBanSuperAdmin_ShouldReturnForbidden()
        {
            var superAdminId = 1;
            var adminToken = _tokenHelper.GenerateJwtToken(4, "User4Surname4@email.com", "Admin");

            var response = await BanUserRequest(superAdminId, adminToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        private async Task<HttpResponseMessage> BanUserRequest(int userId, string token = "")
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return await _client.PatchAsync($"/authorizationapi/users/ban/{userId}", null);
        }
    }
}
