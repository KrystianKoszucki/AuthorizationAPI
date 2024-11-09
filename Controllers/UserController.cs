using Authorization.Extensions;
using Authorization.Models;
using Authorization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.Controllers
{
    [Route("authorizationapi/users")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public UserController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [Authorize]
        [HttpGet("{userId}")]
        public ActionResult GetUser([FromRoute] int userId)
        {
            if (userId != null)
            {
                var user = _userService.GetUser(userId);
                return Ok(user);
            }
            return NotFound();
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] User user)
        {
            if (user == null) return BadRequest();

            var checkIfEmailIsTaken = _userService.GetUserByEmail(user.Email);

            if (checkIfEmailIsTaken != null) return BadRequest();

            _userService.Register(user);
            return Ok();
        }

        [HttpPost("login")]
        public ActionResult LoginUser([FromBody] LoginModel loginData)
        {
            var jwt = _tokenService.GenerateTokenWhileLogging(loginData);
            if(jwt == null || jwt == string.Empty) return BadRequest();

            return Ok(jwt);
        }

        [Authorize]
        [HttpGet("profile")]
        public ActionResult GetUserProfile()
        {
            int userId = User.GetUserId();
            if (userId == 0) return BadRequest();

            var user = _userService.GetUser(userId);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpPatch("changePassword")]
        public ActionResult UpdateUserPassword([FromBody] UpdatePasswordModel passwordRequest)
        {
            int userId = User.GetUserId();

            if (userId == 0) return BadRequest();
            var user = _userService.GetUser(userId);

            if (user == null) return NotFound();
            if (passwordRequest.OldPassword != user.Password || passwordRequest.OldPassword.IsNullOrEmpty() || passwordRequest.NewPassword.IsNullOrEmpty())
            {
                return BadRequest();
            }

            _userService.UpdatePassword(user, passwordRequest.NewPassword);
            return NoContent();
        }
    }
}
