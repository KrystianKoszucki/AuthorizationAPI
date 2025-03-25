namespace Authorization.Models
{
    public class TokenGenerationRequest
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public UserRoles Role { get; set; }

    };
}
