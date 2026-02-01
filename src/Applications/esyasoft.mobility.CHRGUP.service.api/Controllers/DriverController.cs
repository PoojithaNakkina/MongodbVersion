using esyasoft.mobility.CHRGUP.service.api.DTOs.Driver;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Models;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/drivers")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;

        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDriverDto dto)
        {
            var driver = await _driverService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _driverService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _driverService.GetByIdAsync(id));
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, UpdateDriverStatusDto dto)
        {
            await _driverService.UpdateStatusAsync(id, dto.Status);
            return NoContent();
        }

        [HttpPatch("{id}/assign-vehicle")]
        public async Task<IActionResult> AssignVehicle(string id, AssignVehicleDto dto)
        {
            await _driverService.AssignVehicleAsync(id, dto.VehicleId);
            return NoContent();
        }
    }
}
