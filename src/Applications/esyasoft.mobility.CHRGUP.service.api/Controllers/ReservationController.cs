using esyasoft.mobility.CHRGUP.service.api.DTOs.Reservation;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            var reservation = await _service.CreateAsync(dto);
            return Ok(reservation);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(
            string id,
            [FromBody] CancelReservationDto dto)
        {
            var reservation = await _service.CancelAsync(id, dto);
            if (reservation == null) return NotFound();

            return Ok(reservation);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }
    }
}
