using Authorization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Controllers
{
    [Authorize]
    [Route("authorizationapi/user")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getUser/{userId}")]
        public ActionResult GetUser([FromRoute] int userId)
        {
            if (userId != null)
            {
                var user = _userService.GetUser(userId);
                return Ok(user);
            }
            return NotFound();
        }

        [AllowAnonymous]
        [HttpPost("registerUser")]
        public ActionResult RegisterUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            _userService.Register(user);
            return Ok();
        }
    }
}
