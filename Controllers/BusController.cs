using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TravelLove.DTO;
using TravelLove.DTOs;
using TravelLove.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TravelLove.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly TravelLoveDbContext _context;

        public BusController(TravelLoveDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bus>>> GetBuses()
        {
            try
            {
                // Get the list of buses
                var buses = await _context.Buses.ToListAsync();

                foreach (var bus in buses)
                {
                    // Find the corresponding BusSeat based on BusSeatID
                    bus.BusSeat = await _context.BusSeats.FindAsync(bus.BusID);
                }

                return Ok(buses); // Return an HTTP 200 OK response with the list of buses
            }
            catch (Exception ex)
            {
                // Handle the exception as needed, e.g., log it
                return StatusCode(500, "Internal Server Error");
            }
        }
   





        // POST: api/Bus/AddAndGetID
        [HttpPost("AddAndGetID")]
        public async Task<ActionResult<int>> AddBusAndGetID(Bus bus)
        {
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            // Return the newly generated busID
            return bus.BusID;
        }


        // GET: api/Bus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bus>> GetBus(int id)
        {
            var bus = await _context.Buses.FindAsync(id);

            if (bus == null)
            {
                return NotFound();
            }

            return bus;
        }

        // PUT: api/Bus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBus(int id, Bus bus)
        {
            if (id != bus.BusID)
            {
                return BadRequest();
            }

            _context.Entry(bus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusExists(id))
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

        // POST: api/Bus
        [HttpPost]
        public async Task<ActionResult<Bus>> PostBus(Bus bus)
        {
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBus", new { id = bus.BusID }, bus);
        }

        // DELETE: api/Bus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BusExists(int id)
        {
            return _context.Buses.Any(e => e.BusID == id);
        }



        [HttpGet("getAllBusesWithSchedulesAndDrivers")]
        public async Task<ActionResult<List<BusAssignedDataDTO>>> GetAllBusesWithSchedulesAndDrivers()
        {
            var busAssignDataDTOs = new List<BusAssignedDataDTO>();

            try
            {
                // Fetch all buses
                var busesActionResult = await GetBuses();

                if (busesActionResult.Result is OkObjectResult okObjectResult)
                {
                    var buses = okObjectResult.Value as List<Bus>;

                    foreach (var bus in buses)
                    {
                        // Fetch schedules for each bus based on busID
                        var schedulesActionResult = await GetSchedulesByBusID(bus.BusID);

                        if (schedulesActionResult.Result is OkObjectResult schedulesObjectResult)
                        {
                            var schedulesList = new List<Schedule>();

                            var schedules = schedulesObjectResult.Value as List<Schedule>;

                            foreach (var schedule in schedules)
                            {
                                schedule.BusPath = await GetBusPath(schedule.PathID);
                                schedule.Driver = await GetDriver(schedule.DriverID);

                                // Map the data to the DTO

                                schedulesList.Add(schedule);
                            }


                            var busAssignDataDTO = new BusAssignedDataDTO
                            {
                                Bus = bus,
                                Schedules = schedulesList,

                            };
                            busAssignDataDTOs.Add(busAssignDataDTO);
                        }
                    }

                    return busAssignDataDTOs;
                }
                else
                {
                    return StatusCode(500, "Internal Server Error");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed, e.g., log it
                return StatusCode(500, "Internal Server Error");
            }
        }




        // Endpoint to fetch bus assignment data by bus ID
        [HttpGet("getBusAssignData/{busID}")]
        public async Task<ActionResult<List<BusAssignedDataDTO>>> GetBusAssignData(int busID)
        {

            var busAssignDataDTOs = new List<BusAssignedDataDTO>();

            // Fetch data from the Bus controller
            var busActionResult = await GetBus(busID);

            if (busActionResult.Result is OkObjectResult okObjectResult)
            {
                var bus = okObjectResult.Value as Bus;
                var schedulesActionResult = await GetSchedulesByBusID(bus.BusID);


                if (schedulesActionResult.Result is OkObjectResult schedulesObjectResult)
                {
                    var schedulesList = new List<Schedule>();

                    var schedules = schedulesObjectResult.Value as List<Schedule>;

                    foreach (var schedule in schedules)
                    {
                      

                        schedule.BusPath = await GetBusPath(schedule.PathID);
                        schedule.Driver = await GetDriver(schedule.DriverID);

                        // Map the data to the DTO

                        schedulesList.Add(schedule);
                      

                    }

                    var busAssignDataDTO = new BusAssignedDataDTO
                    {
                        Bus = bus,
                        Schedules = schedulesList,

                    };
                    busAssignDataDTOs.Add(busAssignDataDTO);
                }

                return busAssignDataDTOs;
            }
            else
            {
                return null; // Return the original action result if it's not successful
            }
        }

        // Helper method to fetch a driver by driverID
        private async Task<Driver> GetDriver(int driverID)
        {
            return await _context.Drivers.FindAsync(driverID);
        }

        // Helper method to fetch a schedule by busID
        private async Task<Schedule> GetScheduleByBusID(int busID)
        {
            return await _context.Schedules.FirstOrDefaultAsync(s => s.BusID == busID);
        }

        // Helper method to fetch a bus path by pathID
        private async Task<BusPath> GetBusPath(int pathID)
        {
            return await _context.BusPaths.FindAsync(pathID);
        }

        // Get schedules by bus ID
        [HttpGet("GetSchedulesByBusID/{busID}")]
        public async Task<ActionResult<List<Schedule>>> GetSchedulesByBusID(int busID)
        {
            try
            {
                // Retrieve schedules for the specified busID
                var schedules = await _context.Schedules
                    .Where(schedule => schedule.BusID == busID)
                    .ToListAsync();

                if (schedules == null || schedules.Count == 0)
                {
                    return NotFound(); // Return a 404 Not Found response if no schedules are found.
                }

                return Ok(schedules); // Return the list of schedules as a JSON response.
            }
            catch (Exception ex)
            {
                // Handle the exception as needed, e.g., log it
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error response on exception.
            }
        }











    }

    // Similarly, create controllers for Driver, Route, Schedule, Passenger, CreditCard, Ticket, and Review entities.
}
