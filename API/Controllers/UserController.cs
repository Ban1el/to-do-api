using API.DTOs.User;
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
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailDto>> GetById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<UserDetailDto>> Create([FromBody] UserCreateDto dto)
        {
            int userId = User.GetUserId();
            var result = await _userService.CreateAsync(userId, dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDetailDto>> Update(int id, [FromBody] UserUpdateDto dto)
        {
            int userId = User.GetUserId();
            var result = await _userService.UpdateAsync(userId, id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = User.GetUserId();
            await _userService.DeleteAsync(userId, id);
            return Ok();
        }
    }
}
