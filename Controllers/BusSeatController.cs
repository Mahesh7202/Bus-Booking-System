using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLove.DTO.Requests;
using TravelLove.DTOs;
using TravelLove.Models;

namespace TravelLove.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusSeatController : ControllerBase
    {
        private readonly TravelLoveDbContext _context;

        public BusSeatController(TravelLoveDbContext context)
        {
            _context = context;
        }

        // GET: api/BusSeat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusSeat>>> GetBusSeats()
        {
            return await _context.BusSeats.ToListAsync();
        }

        // GET: api/BusSeat/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BusSeat>> GetBusSeat(int id)
        {
            var busSeat = await _context.BusSeats.FindAsync(id);

            if (busSeat == null)
            {
                return NotFound();
            }

            return busSeat;
        }

        // PUT: api/BusSeat/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusSeat(int id, BusSeat busSeat)
        {
            if (id != busSeat.BusSeatID)
            {
                return BadRequest();
            }

            _context.Entry(busSeat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusSeatExists(id))
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

        // POST: api/BusSeat
        [HttpPost]
        public async Task<ActionResult<BusSeat>> PostBusSeat(BusSeat busSeat)
        {
            _context.BusSeats.Add(busSeat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBusSeat", new { id = busSeat.BusSeatID }, busSeat);
        }

        // DELETE: api/BusSeat/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusSeat(int id)
        {
            var busSeat = await _context.BusSeats.FindAsync(id);
            if (busSeat == null)
            {
                return NotFound();
            }

            _context.BusSeats.Remove(busSeat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/BusSeat/ReduceSeats
        [HttpPut("ReduceSeats")]
        public async Task<IActionResult> ReduceSeats(SeatIncreaseorReduceRequest seatIncreaseorReduceRequest)
        {
            try
            {
                var busSeats = await _context.BusSeats
                    .Where(bs => bs.BusID == seatIncreaseorReduceRequest.BusId && seatIncreaseorReduceRequest.SeatTypes.Contains(bs.SeatType))
                    .ToListAsync();

                if (busSeats.Count == 0)
                {
                    return NotFound("No matching bus seats found.");
                }

                foreach (var seatType in seatIncreaseorReduceRequest.SeatTypes)
                {
                    var matchingSeat = busSeats.FirstOrDefault(bs => bs.SeatType == seatType);

                    if (matchingSeat != null && matchingSeat.SeatsAvailable > 0)
                    {
                        matchingSeat.SeatsAvailable--;
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

        // PUT: api/BusSeat/IncreaseSeats
        [HttpPut("IncreaseSeats")]
        public async Task<IActionResult> IncreaseSeats(SeatIncreaseorReduceRequest seatIncreaseorReduceRequest)
        {

            try
            {
                var busSeats = await _context.BusSeats
                    .Where(bs => bs.BusID == seatIncreaseorReduceRequest.BusId && seatIncreaseorReduceRequest.SeatTypes.Contains(bs.SeatType))
                    .ToListAsync();

                if (busSeats.Count == 0)
                {
                    return NotFound("No matching bus seats found.");
                }

                foreach (var seatType in seatIncreaseorReduceRequest.SeatTypes)
                {
                    var matchingSeat = busSeats.FirstOrDefault(bs => bs.SeatType == seatType);

                    if (matchingSeat != null)
                    {
                        matchingSeat.SeatsAvailable++; // Increase seat availability
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




        // Add this action method to the BusSeatController

        // POST: api/BusSeat/AddSeats
        [HttpPost("AddSeats")]
        public async Task<IActionResult> AddSeats([FromBody] BusSeatAddRequestDTO request)
        {
            // Check if the request is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the bus by ID
            var bus = await _context.Buses.FindAsync(request.BusID);

            if (bus == null)
            {
                return NotFound($"Bus with ID {request.BusID} not found.");
            }

            // Add seats to the bus
            foreach (var seatType in request.SeatTypes)
            {
                var busSeat = new BusSeat
                {
                    BusID = request.BusID,
                    SeatType = seatType.SeatType,
                    TotalSeats = seatType.TotalSeats,
                    SeatsAvailable = seatType.TotalSeats // Initially, all seats are available
                };

                _context.BusSeats.Add(busSeat);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBusSeats", new { id = request.BusID }, request);
        }

        // GET: api/BusSeat/ByBus/{busID}
        [HttpGet("ByBus/{busID}")]
        public async Task<ActionResult<IEnumerable<BusSeat>>> GetBusSeatsByBus(int busID)
        {
            // Query the database to get bus seats associated with the specified busID
            var busSeats = await _context.BusSeats
                .Where(seat => seat.BusID == busID)
                .ToListAsync();

            if (busSeats == null || busSeats.Count == 0)
            {
                return NotFound();
            }

            return busSeats;
        }


        private bool BusSeatExists(int id)
        {
            return _context.BusSeats.Any(e => e.BusSeatID == id);
        }
    }
}
