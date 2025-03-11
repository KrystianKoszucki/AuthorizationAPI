using Authorization.Models;
namespace Authorization.Services
{
    public interface IFakeDatabaseService
    {
        void AddUser(User user);
        void DeleteUser(int id);
        List<User> GetAllUsers();
        User GetUserById(int id);
        User GetUserByEmail(string email);
        void UpdateUser(User user);
    }

    public class FakeDatabaseService : IFakeDatabaseService
    {
        public List<User> _users;
        public FakeDatabaseService()
        {
            _users = new List<User>()
            {
                new User() { Id = 1, Name = "User1", Surname = "Surname1", DateOfBirth = new DateTime(1998, 5, 5), Email = "User1Surname1@email.com", Password = "testUser1Surname1", RoleId = UserRoles.SuperAdmin },
                new User() { Id = 2, Name = "User2", Surname = "Surname2", DateOfBirth = new DateTime(2006, 10, 8), Email = "User2Surname2@email.com", Password = "testUser2Surname2", RoleId = UserRoles.User },
                new User() { Id = 3, Name = "User3", Surname = "Surname3", DateOfBirth = new DateTime(2006, 7, 9), Email = "User3Surname3@email.com", Password = "testUser3Surname3", RoleId = UserRoles.Banned },
                new User() { Id = 4, Name = "User4", Surname = "Surname4", DateOfBirth = new DateTime(1980, 10, 10), Email = "User4Surname4@email.com", Password = "testUser4Surname4", RoleId = UserRoles.Admin },
                new User() { Id = 5, Name = "User5", Surname = "Surname5", DateOfBirth = new DateTime(1981, 11, 11), Email = "User5Surname5@email.com", Password = "testUser5Surname5" },

            };
        }

        public void AddUser(User user)
        {
            user.Id = _users.Count + 1;
            _users.Add(user);
        }

        public List<User> GetAllUsers()
        {
            return new List<User>(_users);
        }

        public User GetUserById(int id)
        {
            return _users.Find(u => u.Id == id);
        }

        public User GetUserByEmail(string email)
        {
            return _users.Find(u => u.Email == email);
        }

        public void UpdateUser(User user)
        {
            var existingUser = GetUserById(user.Id);
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                existingUser.Surname = user.Surname;
                existingUser.DateOfBirth = user.DateOfBirth;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
            }
        }

        public void DeleteUser(int id)
        {
            var user = GetUserById(id);
            if (user != null)
            {
                _users.Remove(user);
            }
        }
    }
}
