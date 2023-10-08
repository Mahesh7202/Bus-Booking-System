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
    public class PassengerController : ControllerBase
    {
        private readonly TravelLoveDbContext _context;

        public PassengerController(TravelLoveDbContext context)
        {
            _context = context;
        }

        // GET: api/Passenger
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetPassengers()
        {
            return await _context.Passengers.ToListAsync();
        }

        // GET: api/Passenger/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Passenger>> GetPassenger(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);

            if (passenger == null)
            {
                return NotFound();
            }

            return passenger;
        }

        // PUT: api/Passenger/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPassenger(int id, Passenger passenger)
        {
            if (id != passenger.PassengerID)
            {
                return BadRequest();
            }

            _context.Entry(passenger).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PassengerExists(id))
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

        // POST: api/Passenger
        [HttpPost]
        public async Task<ActionResult<Passenger>> PostPassenger(Passenger passenger)
        {
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPassenger", new { id = passenger.PassengerID }, passenger);
        }

        // DELETE: api/Passenger/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassenger(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null)
            {
                return NotFound();
            }

            _context.Passengers.Remove(passenger);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Passenger/AddPassengers
        [HttpPost("AddPassengers")]
        public async Task<ActionResult<List<int>>> AddPassengers(PassengerListRequest request)
        {
            if (request?.Passengers == null || !request.Passengers.Any())
            {
                return BadRequest("No passengers provided.");
            }

            var passengerIds = new List<int>();

            foreach (var passenger in request.Passengers)
            {
                _context.Passengers.Add(passenger);
                await _context.SaveChangesAsync();
                passengerIds.Add(passenger.PassengerID);
            }

            return Ok(passengerIds);
        }

        private bool PassengerExists(int id)
        {
            return _context.Passengers.Any(e => e.PassengerID == id);
        }
    }

   
 
}
