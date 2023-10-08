using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLove.DTO.Requests;
using TravelLove.Models;

namespace TravelLove.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly TravelLoveDbContext _context;

        public SeatController(TravelLoveDbContext context)
        {
            _context = context;
        }

        // GET: api/Seat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeats()
        {
            return await _context.Seats.ToListAsync();
        }

        // GET: api/Seat/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Seat>> GetSeat(int id)
        {
            var seat = await _context.Seats.FindAsync(id);

            if (seat == null)
            {
                return NotFound();
            }

            return seat;
        }

        // PUT: api/Seat/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSeat(int id, Seat seat)
        {
            if (id != seat.SeatID)
            {
                return BadRequest();
            }

            _context.Entry(seat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Seat
        [HttpPost]
        public async Task<ActionResult<Seat>> PostSeat(Seat seat)
        {
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSeat", new { id = seat.SeatID }, seat);
        }

        // DELETE: api/Seat/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat == null)
            {
                return NotFound();
            }

            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Seat/Book
        [HttpPut("Book")]
        public async Task<IActionResult> BookSeats(SeatUpdateRequest seatUpdateRequest)
        {
            try
            {
                // Find all seats that match the given scheduleId and seatNumbers
                var seatsToBook = await _context.Seats
                    .Where(s => s.ScheduleID == seatUpdateRequest.scheduleId && s.BusID == seatUpdateRequest.busId && seatUpdateRequest.seatNumbers.Contains(s.SeatNumber))
                    .ToListAsync();

                if (seatsToBook == null || seatsToBook.Count == 0)
                {
                    return NotFound("No matching seats found.");
                }

                foreach (var seat in seatsToBook)
                {
                    if (seat.status != "booked")
                    {
                        seat.status = "booked";
                    }
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/Seat/CancelBooking
        [HttpDelete("CancelBooking")]
        public async Task<IActionResult> CancelBooking(int scheduleId, int busId, List<string> seatNumbers)
        {
            try
            {
                var seatsToCancel = await _context.Seats
                    .Where(s => s.ScheduleID == scheduleId && s.BusID == busId && seatNumbers.Contains(s.SeatNumber))
                    .ToListAsync();

                if (seatsToCancel == null || seatsToCancel.Count == 0)
                {
                    return NotFound("No matching seats found.");
                }

                _context.Seats.RemoveRange(seatsToCancel);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Seat/GetByScheduleAndBus
        [HttpGet("GetByScheduleAndBus")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeatsByScheduleAndBus(int scheduleId, int busId)
        {
            try
            {
                var seats = await _context.Seats
                    .Where(s => s.ScheduleID == scheduleId && s.BusID == busId)
                    .ToListAsync();

                if (seats == null || seats.Count == 0)
                {
                    return NotFound("No matching seats found.");
                }

                return Ok(seats);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        private bool SeatExists(int id)
        {
            return _context.Seats.Any(e => e.SeatID == id);
        }
    }
}
