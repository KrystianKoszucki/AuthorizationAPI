using Authorization.Models;

namespace Authorization.Services
{
    public interface IUserService
    {
        void Register(User user);
        User GetUser(int id);
        User GetUserByEmail(string email);
        bool CheckPassword(LoginModel loginModel);
        void UpdatePassword(User user, string newPassword);
    }
    public class UserService: IUserService
    {
        private readonly IFakeDatabaseService _databaseService;

        public UserService(IFakeDatabaseService fakeDatabaseService)
        {
            _databaseService = fakeDatabaseService;
        }

        public void Register(User user)
        {
            var newUser = new User()
            {
                Name = user.Name,
                Surname = user.Surname,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                Password = user.Password
            };

            _databaseService.AddUser(newUser);
        }

        public User GetUser(int id)
        {
            return _databaseService.GetUserById(id);
        }

        public User GetUserByEmail(string email)
        {
            return _databaseService.GetUserByEmail(email);
        }

        public bool CheckPassword(LoginModel loginModel)
        {
            var user = GetUserByEmail(loginModel.Email);

            if (user == null) return false;
            if (loginModel.Password != user.Password) return false;

            return true;

        }

        public void UpdatePassword(User user, string newPassword)
        {
            user.Password = newPassword;
            _databaseService.UpdateUser(user);
        }
    }
}
