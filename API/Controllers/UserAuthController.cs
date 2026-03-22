using API.Attributes;
using API.Constants;
using API.DTOs.User;
using API.Enums;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/user/auth")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly UserAuthService _userAuthService;
        public UserAuthController(UserAuthService userAuthService)
        {
            _userAuthService = userAuthService;
        }

        [HttpPost("signin")]
        [AuditTrail(Module = ModuleConstants.UserAuthentication, Action = ActionConstants.Login)]
        public async Task<ActionResult<UserDto>> SignIn([FromBody] UserSigninDto dto)
        {
            var result = await _userAuthService.SignInAsync(dto);
            return Ok(result);
        }
    }
}
