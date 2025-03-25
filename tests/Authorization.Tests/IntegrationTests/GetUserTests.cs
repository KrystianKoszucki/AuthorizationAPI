using Microsoft.AspNetCore.Http;

namespace Authorization.Tests.IntegrationTests
{
    public class GetUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public GetUserTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUser_AdminAuthenticated_ReturnsOk()
        {
            var client = _client.WithAuthentication("Admin");

            var response = await client.GetAsync("/authorizationapi/users/5");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"id\":5", responseString);
        }

        [Fact]
        public async Task GetUser_UserAuthenticated_Returns403Forbidden()
        {
            var client = _client.WithAuthentication("User");

            var response = await client.GetAsync("/authorizationapi/users/5");

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(StatusCodes.Status403Forbidden, (int)response.StatusCode);
        }

        [Fact]
        public async Task GetUser_UserAuthenticated_Returns404NotFound()
        {
            var client = _client.WithAuthentication("Admin");

            var response = await client.GetAsync("/authorizationapi/users/5000");

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }
    }
}
