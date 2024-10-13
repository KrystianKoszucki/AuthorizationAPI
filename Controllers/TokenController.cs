using Authorization.Models;
using Authorization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Authorization.Controllers
{
    [Route("authorizationapi/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }


        [HttpPost("token")]
        public ActionResult GenerateToken([FromBody]TokenGenerationRequest request)
        {
            if (request == null) return BadRequest();
            
            var jwt = _tokenService.GenerateToken(request);
            if (jwt == null) return NotFound();

            return Ok(jwt);
        }
    }
}
