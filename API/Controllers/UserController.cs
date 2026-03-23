using API.Attributes;
using API.Constants;
using API.DTOs.User;
using API.Enums;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AuditTrail(Module = ModuleConstants.User, Action = ActionConstants.Read)]
        public async Task<ActionResult<UserDetailDto>> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AuditTrail(Module = ModuleConstants.User, Action = ActionConstants.Read)]
        public async Task<ActionResult<UserDetailDto>> GetById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [AuditTrail(Module = ModuleConstants.User, Action = ActionConstants.Create)]
        public async Task<ActionResult<UserDetailDto>> Create([FromBody] UserCreateDto dto)
        {
            int userId = User.GetUserId();
            var result = await _userService.CreateAsync(userId, dto);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            HttpContext.Items[AuditTrailConstants.ReferenceId] = result.Data?.Id;

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        [AuditTrail(Module = ModuleConstants.User, Action = ActionConstants.Update)]
        public async Task<ActionResult<UserDetailDto>> Update(int id, [FromBody] UserUpdateDto dto)
        {
            int userId = User.GetUserId();
            var result = await _userService.UpdateAsync(userId, id, dto);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else if (result.Data == null)
            {
                return NoContent();
            }

            HttpContext.Items[AuditTrailConstants.ReferenceId] = result.Data?.Id;

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        [AuditTrail(Module = ModuleConstants.User, Action = ActionConstants.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = User.GetUserId();
            var result = await _userService.DeleteAsync(userId, id);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
