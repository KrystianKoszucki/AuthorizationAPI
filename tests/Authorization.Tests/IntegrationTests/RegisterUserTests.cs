using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace Authorization.Tests.IntegrationTests
{
    public class RegisterUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public RegisterUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterUser_ShouldReturn200Ok()
        {
            var registerModel = new User
            {
                Name = "newUser",
                Surname = "newUser",
                DateOfBirth = new DateTime(2000, 4, 14),
                Email = "newuser@test.com",
                Password = "newuser123"
            };

            var json = JsonSerializer.Serialize(registerModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/authorizationapi/users/register", content);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnForbiddenCausedByAlreadyUsedEmail()
        {
            var registerModel = new User
            {
                Name = "duplicated",
                Surname = "duplicated",
                DateOfBirth = new DateTime(2000, 4, 14),
                Email = "test@test.com",
                Password = "newuserduplicated123"
            };

            var json = JsonSerializer.Serialize(registerModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/authorizationapi/users/register", content);
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
            Assert.Equal("{\"message\":\"Email is already taken.\"}", responseString);
        }
    }
}
