using API.DTOs.Buggy;
using API.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/buggy")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        [HttpGet("test-log")]
        public async Task<IActionResult> TestLog()
        {
            throw new Exception("Test error message");
        }

        [HttpPost("test-log-post")]
        public async Task<IActionResult> TestLog([FromBody] BuggyCreateDto dto)
        {
            throw new Exception("Test error message");
        }

        [HttpPut("test-log-post/{id}")]
        public async Task<IActionResult> TestLog(int id, [FromBody] BuggyUpdateDto dto)
        {
            throw new Exception("Test error message");
        }
    }
}
