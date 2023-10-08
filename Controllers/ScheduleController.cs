using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class ScheduleController : ControllerBase
    {
        private readonly TravelLoveDbContext _context;

        public ScheduleController(TravelLoveDbContext context)
        {
            _context = context;
        }

        // GET: api/Schedule
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedules()
        {
            var schedules = await _context.Schedules.ToListAsync();
            foreach (var schedule in schedules)
            {
                schedule.BusPath = await _context.BusPaths.FindAsync(schedule.PathID);
            }

            return schedules;

        }

        // GET: api/Schedule/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
            {
                return NotFound();
            }

            return schedule;
        }


        [HttpPost("get-schedules-by-ids")]
        public async Task<ActionResult<List<Schedule>>> GetSchedulesByIds(ScheduleIdsRequest request)
        {
            var schedules = await _context.Schedules
                .Where(schedule => request.ScheduleIds.Contains(schedule.ScheduleID))
                .ToListAsync();

            if (schedules == null || schedules.Count == 0)
            {
                return NotFound();
            }

            return schedules;
        }


        // PUT: api/Schedule/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSchedule(int id, Schedule schedule)
        {
            if (id != schedule.ScheduleID)
            {
                return BadRequest();
            }

            _context.Entry(schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(id))
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

        // POST: api/Schedule
        [HttpPost]
        public async Task<ActionResult<Schedule>> PostSchedule(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSchedule", new { id = schedule.ScheduleID }, schedule);
        }

        // DELETE: api/Schedule/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("ByBusId/{busID}")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedulesByBusID(int busID)
        {
            var schedules = await _context.Schedules
                .Where(schedule => schedule.BusID == busID)
                .ToListAsync();

            if (schedules == null || schedules.Count == 0)
            {
                return NotFound();
            }

            // Load related entities for each schedule
            foreach (var schedule in schedules)
            {
                schedule.Bus = await GetBus(schedule.BusID);
                schedule.BusPath = await GetBusPath(schedule.PathID);
                schedule.Driver = await GetDriver(schedule.DriverID);
            }

            return schedules;
        }

      

        // GET: api/Schedule/ByBusAndPath/{busID}/{pathID}
        [HttpGet("ByBusAndPath/{busID}/{pathID}")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedulesByBusAndPath(int busID, int pathID)
        {
            // Query the database to get schedules associated with the specified busID and pathID
            var schedules = await _context.Schedules
                .Where(schedule => schedule.BusID == busID && schedule.PathID == pathID)
                .ToListAsync();

            if (schedules == null || schedules.Count == 0)
            {
                return NotFound();
            }

            // Fetch associated Bus and BusPath data for each schedule
            foreach (var schedule in schedules)
            {
                schedule.Bus = await _context.Buses.FindAsync(schedule.BusID);
                schedule.BusPath = await _context.BusPaths.FindAsync(schedule.PathID);
            }

            return schedules;
        }

        // GET: api/Schedule/ByBusPath/{busPathID}
        [HttpGet("ByBusPath/{busPathID}")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedulesByBusPath(int busPathID)
        {
            // Query the database to get schedules associated with the specified busPathID
            var schedules = await _context.Schedules
                .Where(schedule => schedule.PathID == busPathID)
                .ToListAsync();

            if (schedules == null || schedules.Count == 0)
            {
                return NotFound();
            }
            

            // Fetch associated Bus data for each schedule
            foreach (var schedule in schedules)
            {
                schedule.Bus = await _context.Buses.FindAsync(schedule.BusID);
                schedule.BusPath = await GetBusPath(schedule.PathID);
                schedule.Driver = await GetDriver(schedule.DriverID);
            }

            return schedules;
        }

        // GET: api/Schedule/ByBusPathAndArrivalDate/{busPathID}/{arrivalDate}
        [HttpGet("ByBusPathAndArrivalDate/{busPathID}/{arrivalDate}")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedulesByBusPathAndArrivalDate(int busPathID, string arrivalDate)
        {
            // Validate the date format
            if (!DateTime.TryParseExact(arrivalDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return BadRequest("Invalid date format. Please use yyyy-MM-dd format.");
            }

            // Query the database to get schedules associated with the specified busPathID and arrivalDate
            var schedules = await _context.Schedules
                .Where(schedule => schedule.PathID == busPathID && schedule.ArrivalTime.Date == parsedDate.Date)
                .ToListAsync();

            if (schedules == null || schedules.Count == 0)
            {
                return NotFound();
            }

            // Fetch associated Bus data for each schedule
            foreach (var schedule in schedules)
            {
                schedule.Bus = await _context.Buses.FindAsync(schedule.BusID);
                schedule.BusPath = await _context.BusPaths.FindAsync(schedule.PathID);
                schedule.Driver = await _context.Drivers.FindAsync(schedule.DriverID);

            }

            return schedules;
        }

        private async Task<Bus> GetBus(int busID)
        {
            // Implement logic to retrieve Bus entity by busID from your database
            return await _context.Buses.FindAsync(busID);
        }

        private async Task<BusPath> GetBusPath(int pathID)
        {
            // Implement logic to retrieve BusPath entity by pathID from your database
            return await _context.BusPaths.FindAsync(pathID);
        }

        private async Task<Driver> GetDriver(int driverID)
        {
            // Implement logic to retrieve Driver entity by driverID from your database
            return await _context.Drivers.FindAsync(driverID);
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ScheduleID == id);
        }
    }

}