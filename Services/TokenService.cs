﻿using Authorization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;

namespace Authorization.Services
{
    public interface ITokenService
    {
        string GenerateToken(TokenGenerationRequest request);
    }
    public class TokenService: ITokenService
    {
        private readonly JwtSettings _options;
        private static readonly TimeSpan TokenExpires = TimeSpan.FromHours(8);

        public TokenService(IOptions<JwtSettings> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(TokenGenerationRequest request)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_options.Key);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, request.Email),
                new(JwtRegisteredClaimNames.Email, request.Email),
                new("id", request.Id.ToString())
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
