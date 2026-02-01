using esyasoft.mobility.CHRGUP.service.api.DTOs.Charger;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/chargers")]
    public class ChargerController : ControllerBase
    {
        private readonly IChargerService _chargerService;

        public ChargerController(IChargerService chargerService)
        {
            _chargerService = chargerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _chargerService.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] ChargerRegisterDto dto)
        {
            return Ok(await _chargerService.RegisterAsync(dto.locationId, dto.version));
        }

        [HttpPut("{chargerId}/status")]
        public async Task<IActionResult> UpdateStatus(
            string chargerId,
            [FromQuery] ChargerStatus status)
        {
            await _chargerService.UpdateStatusAsync(chargerId, status);
            return NoContent();
        }

        [HttpPut("{chargerId}/heartbeat")]
        public async Task<IActionResult> Heartbeat(string chargerId)
        {
            await _chargerService.UpdateHeartbeatAsync(chargerId, DateTime.Now);
            return NoContent();
        }

        
    }
}
