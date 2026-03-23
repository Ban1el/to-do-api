using API.Attributes;
using API.Constants;
using API.DTOs;
using API.Enums;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/to-do")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoService _toDoService;
        public ToDoController(ToDoService toDoService)
        {
            _toDoService = toDoService;
        }

        [HttpGet]
        [AuditTrail(Module = ModuleConstants.Todo, Action = ActionConstants.Read)]
        public async Task<ActionResult<ToDoDto>> GetAll()
        {
            var result = await _toDoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AuditTrail(Module = ModuleConstants.Todo, Action = ActionConstants.Read)]
        public async Task<ActionResult<ToDoDto>> GetById(int id)
        {
            var result = await _toDoService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [AuditTrail(Module = ModuleConstants.Todo, Action = ActionConstants.Create)]
        public async Task<ActionResult<ToDoDto>> Create([FromBody] ToDoCreateDto dto)
        {
            int userId = User.GetUserId();
            var result = await _toDoService.CreateAsync(userId, dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [AuditTrail(Module = ModuleConstants.Todo, Action = ActionConstants.Update)]
        public async Task<ActionResult<ToDoDto>> Update(int id, [FromBody] ToDoUpdateDto dto)
        {
            int userId = User.GetUserId();
            var result = await _toDoService.UpdateAsync(userId, id, dto);

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
        [AuditTrail(Module = ModuleConstants.Todo, Action = ActionConstants.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = User.GetUserId();
            var result = await _toDoService.DeleteAsync(userId, id);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
