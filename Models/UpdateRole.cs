namespace Authorization.Models
{
    public class UpdateRole
    {
        public int UserId { get; set; }
        public UserRoles Role { get; set; }
    }
}
