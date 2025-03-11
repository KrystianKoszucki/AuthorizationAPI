using Authorization.Models;

namespace Authorization.Services
{
    public interface IUserService
    {
        Task Register(User user);
        Task<User> GetUser(int id);
        Task<User> GetUserByEmail(string email);
        Task<bool> CheckPassword(LoginModel loginModel);
        Task UpdatePassword(User user, string newPassword);
        Task UpdateRole(User user, UpdateRole updateRoleRequest);
        Task DeleteUser(int id);
        Task BanUser(User user);
        Task UnbanUser(User user);
        Task PermabanUser(User user);
    }
    public class UserService: IUserService
    {
        private readonly IDatabaseService _database;

        public UserService(IDatabaseService database)
        {
            _database = database;
        }

        public async Task Register(User user)
        {
            var newUser = new User()
            {
                Name = user.Name,
                Surname = user.Surname,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                Password = user.Password,
                RoleId = UserRoles.User
            };

            await _database.AddUserAsync(newUser);
        }

        public async Task<User> GetUser(int id)
        {
            return await _database.GetUserByIdAsync(id);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _database.GetUserByEmailAsync(email);
        }

        public async Task<bool> CheckPassword(LoginModel loginModel)
        {
            var user = await GetUserByEmail(loginModel.Email);

            if (user == null) return false;
            if (loginModel.Password != user.Password) return false;

            return true;
        }

        public async Task UpdatePassword(User user, string newPassword)
        {
            user.Password = newPassword;
            await _database.UpdateUserAsync(user);
        }

        public async Task UpdateRole(User user, UpdateRole updateRoleRequest)
        {
            user.RoleId = updateRoleRequest.Role;
            await _database.UpdateUserAsync(user);
        }

        public async Task DeleteUser(int id)
        {
            await _database.DeleteUserAsync(id);
        }

        public async Task BanUser(User user)
        {
            user.HandleBan();
            await _database.UpdateUserAsync(user);
        }

        public async Task UnbanUser(User user)
        {
            user.Unban();
            await _database.UpdateUserAsync(user);
        }

        public async Task PermabanUser(User user)
        {
            user.Permaban();
            await _database.UpdateUserAsync(user);
        }
    }
}
