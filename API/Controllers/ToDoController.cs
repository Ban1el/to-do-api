using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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
        public async Task<ActionResult<ToDoDto>> GetAll()
        {
            var result = await _toDoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoDto>> GetById(int id)
        {
            var result = await _toDoService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ToDoDto>> Create([FromBody] ToDoCreateDto dto)
        {
            var result = await _toDoService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToDoDto>> Update(int id, [FromBody] ToDoUpdateDto dto)
        {
            var result = await _toDoService.UpdateAsync(id, dto);
            return Ok(result);
        }
    }
}
