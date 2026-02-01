using esyasoft.mobility.CHRGUP.service.api.DTOs.Charger;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Messaging;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/charging-sessions")]
    public class ChargingSessionController : ControllerBase
    {
        private readonly IChargingSessionService _service;

        public ChargingSessionController(IChargingSessionService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetByCharger([FromQuery] string chargerId)
        {
            return Ok(await _service.GetByChargerAsync(chargerId));
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromBody] StartChargingRequestDto req)
        {
            try
            {
                var sessionId = await _service
                .StartSessionAsync(req.ChargerId, req.DriverId);
                return Ok(new { sessionId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopSession([FromBody] StopChargingRequestDto req)
        {
            try
            {
                await _service.StopSessionAsync(req.SessionId);
                return Ok("session stopped");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
