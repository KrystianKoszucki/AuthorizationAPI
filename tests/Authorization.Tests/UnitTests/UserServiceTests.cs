namespace Authorization.Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IDatabaseService> _mockDatabaseService;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _mockDatabaseService = new Mock<IDatabaseService>();
            _userService = new UserService(_mockDatabaseService.Object);
        }

        [Fact]
        public async Task Register_ShouldCallDatabaseServiceToAddUser()
        {
            var user = new User { Name = "Unit", Surname = "Test", Email = "unit.test@email.com", Password = "password123" };

            await _userService.Register(user);

            _mockDatabaseService.Verify(db => db.AddUserAsync(It.Is<User>(u => u.Email == user.Email)), Times.Once);
        }

        [Fact]
        public async Task GetUser_ShouldReturnUser_WhenUserExists()
        {
            var userId = 1;
            var user = new User { Id = userId, Name = "Unit", Surname = "Test" };
            _mockDatabaseService.Setup(db => db.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var result = await _userService.GetUser(userId);
            
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(user.Name, result.Name);
        }

        [Fact]
        public async Task GetUser_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userId = 1;
            _mockDatabaseService.Setup(db => db.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _userService.GetUser(userId);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task CheckPassword_ShouldReturnTrue_WhenPasswordMatches()
        {
            var loginModel = new LoginModel { Email = "unit.test@email.com", Password = "password123" };
            var user = new User { Email = "unit.test@email.com", Password = "password123" };
            _mockDatabaseService.Setup(db => db.GetUserByEmailAsync(loginModel.Email)).ReturnsAsync(user);

            var result = await _userService.CheckPassword(loginModel);

            Assert.True(result);
        }

        [Fact]
        public async Task CheckPassword_ShouldReturnFalse_WhenPasswordDoesNotMatch()
        {
            var loginModel = new LoginModel { Email = "unit.test@email.com", Password = "wrongpassword" };
            var user = new User { Email = "unit.test@email.com", Password = "password123" };
            _mockDatabaseService.Setup(db => db.GetUserByEmailAsync(loginModel.Email)).ReturnsAsync(user);

            var result = await _userService.CheckPassword(loginModel);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdatePassword_ShouldCallDatabaseServiceToUpdateUser()
        {
            var user = new User { Id = 1, Email = "unit.test@email.com", Password = "password123" };
            var newPassword = "newpassword123";

            await _userService.UpdatePassword(user, newPassword);

            Assert.Equal(newPassword, user.Password);
            _mockDatabaseService.Verify(db => db.UpdateUserAsync(It.Is<User>(u => u.Password == newPassword)), Times.Once);
        }

        [Fact]
        public async Task UpdateRole_ShouldCallDatabaseServiceToUpdateUserRole()
        {
            var user = new User { Id = 1, RoleId = UserRoles.User };
            var updateRoleRequest = new UpdateRole { Role = UserRoles.Admin };

            await _userService.UpdateRole(user, updateRoleRequest);

            Assert.Equal(UserRoles.Admin, user.RoleId);
            _mockDatabaseService.Verify(db => db.UpdateUserAsync(It.Is<User>(u => u.RoleId == UserRoles.Admin)), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldCallDatabaseServiceToDeleteUser()
        {
            var userId = 1;

            await _userService.DeleteUser(userId);

            _mockDatabaseService.Verify(db => db.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task BanUser_ShouldCallDatabaseServiceToUpdateUser()
        {
            var user = new User { Name = "Unit", Surname = "Test", Email = "unit.test@email.com", Password = "password123" };

            await _userService.BanUser(user);

            Assert.True(user.IsCurrentlyBanned());
            _mockDatabaseService.Verify(db => db.UpdateUserAsync(It.Is<User>(u => u.IsCurrentlyBanned())), Times.Once);
        }

        [Fact]
        public async Task UnbanUser_ShouldCallDatabaseServiceToUpdateUser()
        {
            var user = new User { Name = "Unit", Surname = "Test", Email = "unit.test@email.com", Password = "password123" };
            await _userService.BanUser(user);
            Assert.True(user.IsCurrentlyBanned());

            await _userService.UnbanUser(user);

            Assert.False(user.IsCurrentlyBanned());
            _mockDatabaseService.Verify(db => db.UpdateUserAsync(It.Is<User>(u => !u.IsCurrentlyBanned())), Times.Between(1, 2, (Moq.Range)2));
        }

        [Fact]
        public async Task PermabanUser_ShouldCallDatabaseServiceToUpdateUser()
        {
            var user = new User { Name = "Unit", Surname = "Test", Email = "unit.test@email.com", Password = "password123" };

            await _userService.PermabanUser(user);

            Assert.True(user.IsPermamentBan());
            _mockDatabaseService.Verify(db => db.UpdateUserAsync(It.Is<User>(u => u.IsPermamentBan())), Times.Once);
        }
    }
}
