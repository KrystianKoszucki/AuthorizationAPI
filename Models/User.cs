namespace Authorization.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRoles RoleId { get; set; }

        public UserRoles RoleIdBeforeBan { get; private set; }
        public int BanCounter { get; private set; } = 0;
        public DateTime LastBanDate { get; private set; }
        public TimeSpan? BanDuration { get; private set; }

        public void HandleBan()
        {
            RoleIdBeforeBan = RoleId;
            ++BanCounter;
            LastBanDate = DateTime.Now;
            BanDuration = GetBanDuration(BanCounter);
            RoleId = UserRoles.Banned;
        }

        private TimeSpan? GetBanDuration(int banCount)
        {
            return banCount switch
            {
                1 => TimeSpan.FromDays(14),
                2 => TimeSpan.FromDays(28),
                _ => null
            };
        }

        public bool IsCurrentlyBanned()
        {
            var bannedPermammently = IsPermamentBan();

            if (bannedPermammently)
            {
                return true;
            }

            if (!BanDuration.HasValue)
            {
                return false;
            }

            if (DateTime.Now >= LastBanDate + BanDuration)
            {
                RoleId = RoleIdBeforeBan;
                return false;
            }

            return true;
        }

        public bool IsPermamentBan()
        {
            return BanDuration == null && BanCounter >= 3;
        }

        public void Unban()
        {
            --BanCounter;
            RoleId = RoleIdBeforeBan;
            BanDuration = TimeSpan.FromDays(0);
        }

        public void Permaban()
        {
            BanCounter = 3;
            RoleId = UserRoles.Banned;
            BanDuration = null;
        }

    }
}