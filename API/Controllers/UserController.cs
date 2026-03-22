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
        public async Task<ActionResult<UserDetailDto>> GetAll()
        {
            HttpContext.Items[AuditTrailConstants.AuditModule] = ModuleConstants.User;
            HttpContext.Items[AuditTrailConstants.AuditAction] = ActionConstants.Read;

            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailDto>> GetById(int id)
        {
            HttpContext.Items[AuditTrailConstants.AuditModule] = ModuleConstants.User;
            HttpContext.Items[AuditTrailConstants.AuditAction] = ActionConstants.Read;

            var result = await _userService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<UserDetailDto>> Create([FromBody] UserCreateDto dto)
        {
            HttpContext.Items[AuditTrailConstants.AuditModule] = ModuleConstants.User;
            HttpContext.Items[AuditTrailConstants.AuditAction] = ActionConstants.Create;

            int userId = User.GetUserId();
            var result = await _userService.CreateAsync(userId, dto);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
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

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
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
