using esyasoft.mobility.CHRGUP.service.api.DTOs.Location;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationDto request)
        {
            return Ok(await _locationService.CreateAsync(request));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _locationService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var location = await _locationService.GetByIdAsync(id);
            return location == null ? NotFound() : Ok(location);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateLocationDto request)
        {
            var location = await _locationService.UpdateAsync(id, request);
            return location == null ? NotFound() : Ok(location);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _locationService.DeleteAsync(id);
                return Ok("Location deleted successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("with-chargers")]
        public async Task<IActionResult> GetAllWithChargers()
        {
            return Ok(await _locationService.GetAllWithChargersAsync());
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Search term is required");

            return Ok(await _locationService.SearchAsync(name));
        }

        [HttpGet("{id}/chargers")]
        public async Task<IActionResult> GetLocationWithChargers(string id)
        {
            var result = await _locationService.GetLocationWithChargersAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
