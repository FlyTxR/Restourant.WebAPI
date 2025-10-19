using Business.DTO;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingsController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings(CancellationToken cancellationToken = default)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(cancellationToken);
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        [HttpGet("status/{statusId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByStatus(int statusId, CancellationToken cancellationToken = default)
        {
            var bookings = await _bookingService.GetBookingsByStatusAsync(statusId, cancellationToken);
            return Ok(bookings);
        }

        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByDate(DateTime date, CancellationToken cancellationToken = default)
        {
            var bookings = await _bookingService.GetBookingsByDateAsync(date, cancellationToken);
            return Ok(bookings);
        }

        [HttpGet("available-slots")]
        public async Task<ActionResult<IEnumerable<TimeSlotDTO>>> GetAvailableTimeSlots(
        [FromQuery] string date,
        [FromQuery] int guests,
        CancellationToken cancellationToken = default)
            {
                if (!DateTime.TryParse(date, out var bookingDate))
                {
                    return BadRequest("Data non valida");
                }

                if (guests <= 0)
                {
                    return BadRequest("Numero di ospiti non valido");
                }

                var slots = await _bookingService.GetAvailableTimeSlotsAsync(bookingDate, guests, cancellationToken);
                return Ok(slots);
            }


        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDTO bookingDto)
        {
            var booking = await _bookingService.CreateBookingAsync(bookingDto);
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, UpdateBookingDTO bookingDto)
        {
            var result = await _bookingService.UpdateBookingAsync(id, bookingDto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id}/status/{statusId}")]
        public async Task<IActionResult> UpdateBookingStatus(int id, int statusId)
        {
            var result = await _bookingService.UpdateBookingStatusAsync(id, statusId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _bookingService.DeleteBookingAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
