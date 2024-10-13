namespace Authorization.Services
{
    public interface IUserService
    {
        void Register(User user);
        User GetUser(int id);
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
    }
}
