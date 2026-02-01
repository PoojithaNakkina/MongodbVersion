using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? sessionId,
            [FromQuery] string? chargerId,
            [FromQuery] string? driverId)
        {
            if (!string.IsNullOrEmpty(sessionId))
                return Ok(await _logService.GetBySessionIdAsync(sessionId));

            if (!string.IsNullOrEmpty(chargerId))
                return Ok(await _logService.GetByChargerIdAsync(chargerId));

            if (!string.IsNullOrEmpty(driverId))
                return Ok(await _logService.GetByDriverIdAsync(driverId));

            return Ok(await _logService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _logService.GetByIdAsync(id));
        }
    }
}
