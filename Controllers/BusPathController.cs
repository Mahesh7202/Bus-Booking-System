using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLove.Models;

namespace TravelLove.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusPathController : ControllerBase
    {
        private readonly TravelLoveDbContext _context;

        public BusPathController(TravelLoveDbContext context)
        {
            _context = context;
        }

        // GET: api/BusPath
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusPath>>> GetBusPaths()
        {
            return await _context.BusPaths.ToListAsync();
        }

        // GET: api/BusPath/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BusPath>> GetBusPath(int id)
        {
            var busPath = await _context.BusPaths.FindAsync(id);

            if (busPath == null)
            {
                return NotFound();
            }

            return busPath;
        }

        // PUT: api/BusPath/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusPath(int id, BusPath busPath)
        {
            if (id != busPath.PathID)
            {
                return BadRequest();
            }

            _context.Entry(busPath).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusPathExists(id))
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

        // POST: api/BusPath
        [HttpPost]
        public async Task<ActionResult<BusPath>> PostBusPath(BusPath busPath)
        {
            _context.BusPaths.Add(busPath);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBusPath", new { id = busPath.PathID }, busPath);
        }

        // DELETE: api/BusPath/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusPath(int id)
        {
            var busPath = await _context.BusPaths.FindAsync(id);
            if (busPath == null)
            {
                return NotFound();
            }

            _context.BusPaths.Remove(busPath);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/BusPath/GetBusPathsByPoints
        [HttpGet("GetBusPathByPoints")]
        public async Task<ActionResult<BusPath>> GetBusPathByPoints(string startingPoint, string endingPoint)
        {
            var busPath = await _context.BusPaths
                .FirstOrDefaultAsync(bp => bp.StartingPoint == startingPoint && bp.EndingPoint == endingPoint);

            if (busPath == null)
            {
                return NotFound();
            }

            return busPath;
        }




        private bool BusPathExists(int id)
        {
            return _context.BusPaths.Any(e => e.PathID == id);
        }
    }
}
