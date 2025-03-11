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

        [Authorize(Policy = "AdminOrSuperAdminPolicy")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser([FromRoute] int userId)
        {
            if (userId != null)
            {
                var user = await _userService.GetUser(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User does not exist" });
                }
                return Ok(user);
            }
            return NotFound();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            if (user == null) return BadRequest();

            var checkIfEmailIsTaken = await _userService.GetUserByEmail(user.Email);

            if (checkIfEmailIsTaken != null) return BadRequest(new { message = "Email is already taken." });

            await _userService.Register(user);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel loginData)
        {
            var jwt = await _tokenService.GenerateTokenWhileLogging(loginData);

            if (jwt == null || jwt == string.Empty)
            {
                return BadRequest("Incorrect e-mail or password");
            }

            if (jwt.StartsWith("Your"))
            {
                return new ObjectResult(jwt)
                {
                    StatusCode = 403
                };
            }

            return Ok(jwt);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            int userId = User.GetUserId();
            if (userId == 0) return BadRequest();

            var user = await _userService.GetUser(userId);

            if (user == null)
            {
                return NotFound(new { message = "User does not exist" });
            }

            if (user.IsCurrentlyBanned())
                {
                var response = new { message = $"Your account has been banned, so viewing profile action is impossible" };
                return new ObjectResult(response)
                {
                    StatusCode = 403
                };
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPatch("changePassword")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UpdatePasswordModel passwordRequest)
        {
            int userId = User.GetUserId();

            if (userId == 0) return BadRequest();
            var user = await _userService.GetUser(userId);

            if (user == null) return NotFound();
            if (passwordRequest.OldPassword != user.Password || passwordRequest.OldPassword.IsNullOrEmpty() || passwordRequest.NewPassword.IsNullOrEmpty())
            {
                return BadRequest();
            }

            await _userService.UpdatePassword(user, passwordRequest.NewPassword);
            return NoContent();
        }

        [Authorize(Policy = "AdminOrSuperAdminPolicy")]
        [HttpPatch("changeRole")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRole updateRoleRequest)
        {
            var user = await _userService.GetUser(updateRoleRequest.UserId);

            if (user == null)
            {
                return NotFound(new { message = "User does not exist" });
            }

            if (user.RoleId == UserRoles.Banned)
            {
                return BadRequest(new { message = "Cannot change banned user's role." });
            }

            if (!Enum.IsDefined(typeof(UserRoles), updateRoleRequest.Role))
            {
                return BadRequest(new { message = "Invalid role provided." });
            }

            if (updateRoleRequest.Role == UserRoles.Banned)
            {
                return BadRequest(new { message = "Cannot ban user." });
            }

            if (user.RoleId == updateRoleRequest.Role)
            {
                var response = new { message = $"Role: {user.RoleId} cannot be changed for role: {updateRoleRequest.Role}" };
                return new ObjectResult(response)
                {
                    StatusCode = 400
                };
            }
            await _userService.UpdateRole(user, updateRoleRequest);
            return NoContent();
        }

        [Authorize(Roles = "Admin,SuperAdmin,User")]
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId)
        {
            if (userId == 0)
            {
                return BadRequest();
            }

            int currentUserId = User.GetUserId();
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && currentUserId != userId)
            {
                var message = "Cannot delete resource.";
                return new ObjectResult(message)
                {
                    StatusCode = 403
                };
            }

            var user = await _userService.GetUser(userId);
            if (user == null)
            {
                return NotFound("User does not exist");
            }

            if (user.IsCurrentlyBanned())
            {
                var message = "Cannot delete banned user.";
                return new ObjectResult(message)
                {
                    StatusCode = 403
                };
            }
            await _userService.DeleteUser(userId);
            return Ok(new { message = "Resource deleted successfully." });
        }

        [Authorize(Policy = "AdminOrSuperAdminPolicy")]
        [HttpPatch("ban/{userId}")]
        public async Task<IActionResult> BanUser([FromRoute] int userId)
        {
            var userToBeBanned = await _userService.GetUser(userId);
            if (userToBeBanned == null)
            {
                return NotFound("User does not exist");
            }

            if (userToBeBanned.IsCurrentlyBanned())
            {
                return BadRequest("User is already banned");
            }

            var currentUserId = User.GetUserId();
            var currentUser = await _userService.GetUser(currentUserId);
            if (currentUser == null)
            {
                var response = new
                {
                    message = "Your account just got probably deleted."
                };
                return new ObjectResult(response)
                {
                    StatusCode = 403
                };
            }

            var triedToSelfHandle = CheckSelfHandlingBanActions(userId, "You cannot ban yourself.");
            if (triedToSelfHandle != null)
            {
                return triedToSelfHandle;
            }

            var isSuperAdmin = currentUser.RoleId == UserRoles.SuperAdmin;
            if (isSuperAdmin)
            {
                await _userService.BanUser(userToBeBanned);
                return NoContent();
            }

            var isUserToBeBannedSuperAdmin = userToBeBanned.RoleId == UserRoles.SuperAdmin;
            if (isUserToBeBannedSuperAdmin)
            {
                var message = "Cannot ban user with SuperAdmin role as a user with Admin role";
                return new ObjectResult(message)
                {
                    StatusCode = 403
                };
            }
            
            await _userService.BanUser(userToBeBanned);
            return NoContent();
        }

        [Authorize(Policy = "AdminOrSuperAdminPolicy")]
        [HttpPatch("unban/{userId}")]
        public async Task<IActionResult> UnbanUser([FromRoute] int userId)
        {
            var userToBeUnbanned = await _userService.GetUser(userId);

            if(userToBeUnbanned == null)
            {
                return NotFound("User does not exist");
            }

            if (userToBeUnbanned.IsPermamentBan())
            {
                var response = new { message = $"User: {userToBeUnbanned.Email} " +
                    $"is permamently banned." };
                return new ObjectResult(response)
                {
                    StatusCode = 403
                };
            }

            if (!userToBeUnbanned.IsCurrentlyBanned())
            {
                return BadRequest("User is not banned");
            }

            var triedToSelfHandle = CheckSelfHandlingBanActions(userId, "You cannot unban yourself.");
            if (triedToSelfHandle != null)
            {
                return triedToSelfHandle;
            }

            await _userService.UnbanUser(userToBeUnbanned);
            return NoContent();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPatch("undoPermaban/{userId}")]
        public async Task<IActionResult> UndoPermaban([FromRoute] int userId)
        {
            var userToBeUnbanned = await _userService.GetUser(userId);
            if (userToBeUnbanned == null)
            {
                return NotFound(new { message = "User does not exist" });
            }

            if (!userToBeUnbanned.IsCurrentlyBanned())
            {
                return BadRequest("User is not banned");
            }

            var triedToSelfHandle = CheckSelfHandlingBanActions(userId, "You cannot undo permaban on yourself.");
            if (triedToSelfHandle != null)
            {
                return triedToSelfHandle;
            }

            await _userService.UnbanUser(userToBeUnbanned);
            return NoContent();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPatch("permaban/{userId}")]
        public async Task<IActionResult> Permaban([FromRoute] int userId)
        {
            var userToBePermabanned = await _userService.GetUser(userId);
            if (userToBePermabanned == null)
            {
                return NotFound(new { message = "User does not exist" });
            }

            var triedToSelfHandle = CheckSelfHandlingBanActions(userId, "You cannot permaban yourself.");
            if (triedToSelfHandle != null)
            {
                return triedToSelfHandle;
            }

            if (userToBePermabanned.IsPermamentBan())
            {
                var message = "User is already permamently banned.";
                return new ObjectResult(message)
                {
                    StatusCode = 403
                };
            }

            await _userService.PermabanUser(userToBePermabanned);
            return NoContent();
        }

        private IActionResult? CheckSelfHandlingBanActions(int userIdWithBanAction, string message)
        {
            var banHandlingUserId = User.GetUserId();
            if (banHandlingUserId == userIdWithBanAction)
            {
                var response = new { message };
                return new ObjectResult(response)
                {
                    StatusCode = 403
                };
            }
            else
            {
                return null;
            }
        }
    }
}
