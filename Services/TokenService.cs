using Authorization.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace Authorization.Services
{
    public interface ITokenService
    {
        string GenerateTokenWhileLogging(LoginModel login);
    }
    public class TokenService: ITokenService
    {
        private readonly JwtSettings _options;
        private static readonly TimeSpan TokenExpires = TimeSpan.FromHours(8);
        private readonly IUserService _userService;

        public TokenService(IOptions<JwtSettings> options, IUserService userService)
        {
            _options = options.Value;
            _userService = userService;
        }

        public string GenerateTokenWhileLogging(LoginModel login)
        {
            var tokenCanBeCreated = _userService.CheckPassword(login);

            if (!tokenCanBeCreated) return "";
            var user = _userService.GetUserByEmail(login.Email);
            var tokenRequest = new TokenGenerationRequest()
            {
                Id = user.Id,
                Email = login.Email,
                Role = user.RoleId
            };
            var jwt = GenerateToken(tokenRequest);
            return jwt;
        }

        internal string GenerateToken(TokenGenerationRequest request)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_options.Key);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, request.Email),
                new(JwtRegisteredClaimNames.Email, request.Email),
                new("id", request.Id.ToString()),
                new("role", request.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenExpires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _options.Audience,
                Issuer = _options.Issuer,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);
            return jwt;
        }
    }
}
