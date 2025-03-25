using Microsoft.Extensions.Options;

namespace Authorization.Tests.UnitTests
{
    public class TokenServiceTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IOptions<JwtSettings>> _mockJwtSettings;
        private readonly ITokenService _tokenService;

        public TokenServiceTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
            _mockJwtSettings.Setup(options => options.Value).Returns(new JwtSettings
            {
                Key = "completelySecureKeyHeHeINeedMoreCharactersHere",
                Audience = "",
                Issuer = ""
            });

            _tokenService = new TokenService(_mockJwtSettings.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task GenerateTokenWhileLogging_ShouldReturnEmptyString_WhenPasswordIsIncorrect()
        {
            var loginModel = new LoginModel { Email = "test@example.com", Password = "wrongpassword" };
            _mockUserService.Setup(u => u.CheckPassword(loginModel)).ReturnsAsync(false);

            var result = await _tokenService.GenerateTokenWhileLogging(loginModel);

            Assert.Equal("", result);
        }

        [Fact]
        public async Task GenerateTokenWhileLogging_ShouldReturnMessage_WhenUserIsPermamentlyBanned()
        {
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password123" };
            var user = new User { Email = "test@example.com", Password = "wrongpassword" };
            user.Permaban();
            _mockUserService.Setup(u => u.CheckPassword(loginModel)).ReturnsAsync(true);
            _mockUserService.Setup(u => u.GetUserByEmail(loginModel.Email)).ReturnsAsync(user);

            var result = await _tokenService.GenerateTokenWhileLogging(loginModel);

            Assert.Equal("Your account has been permamently banned. Contact SuperAdmin if you think you are wrongfully acused", result);
        }

        [Fact]
        public async Task GenerateTokenWhileLogging_ShouldReturnMessage_WhenUserIsCurrentlyBanned()
        {
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password123" };
            var user = new User { Email = "test@example.com", Password = "wrongpassword" };
            user.HandleBan();
            _mockUserService.Setup(u => u.CheckPassword(loginModel)).ReturnsAsync(true);
            _mockUserService.Setup(u => u.GetUserByEmail(loginModel.Email)).ReturnsAsync(user);

            var result = await _tokenService.GenerateTokenWhileLogging(loginModel);

            Assert.Contains("Your account has been banned until", result);
            Assert.Contains(user.BanEndDate.ToString(), result);
        }

        [Fact]
        public async Task GenerateTokenWhileLogging_ShouldReturnToken_WhenUserIsNotBanned()
        {
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password123" };
            var user = new User { Id = 1, Email = "test@example.com", RoleId = UserRoles.User };
            _mockUserService.Setup(u => u.CheckPassword(loginModel)).ReturnsAsync(true);
            _mockUserService.Setup(u => u.GetUserByEmail(loginModel.Email)).ReturnsAsync(user);

            var result = await _tokenService.GenerateTokenWhileLogging(loginModel);

            Assert.NotNull(result);
            Assert.Contains("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", result);
        }
    }
}
