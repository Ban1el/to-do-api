using API.Attributes;
using API.Constants;
using API.DTOs.Buggy;
using API.Enums;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/buggy")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        [HttpGet("test-log")]
        [AuditTrail(Module = ModuleConstants.Buggy, Action = ActionConstants.Read)]
        public async Task<IActionResult> TestLog()
        {
            throw new Exception("Test error message");
        }

        [HttpPost("test-log-post")]
        [AuditTrail(Module = ModuleConstants.Buggy, Action = ActionConstants.Create)]
        public async Task<IActionResult> TestLog([FromBody] BuggyCreateDto dto)
        {
            throw new Exception("Test error message");
        }

        [HttpPut("test-log-post/{id}")]
        [AuditTrail(Module = ModuleConstants.Buggy, Action = ActionConstants.Update)]
        public async Task<IActionResult> TestLog(int id, [FromBody] BuggyUpdateDto dto)
        {
            throw new Exception("Test error message");
        }
    }
}
