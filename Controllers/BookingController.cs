using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TravelLove.DTO.Requests;
using TravelLove.Models;

namespace TravelLove.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly TravelLoveDbContext _context;

        public BookingController(TravelLoveDbContext context)
        {
            _context = context;
        }

        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings.ToListAsync();
        }

        // GET: api/Booking/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // POST: api/Booking
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingID }, booking);
        }

        // POST: api/Booking/AddTicketToBooking
        [HttpPost("AddTicketToBooking")]
        public async Task<ActionResult> AddTicketToBooking(BookingTicket bookingTicket)
        {
            // Check if the Booking and Ticket exist
            var booking = await _context.Bookings.FindAsync(bookingTicket.BookingID);
            var ticket = await _context.Tickets.FindAsync(bookingTicket.TicketID);

            if (booking == null || ticket == null)
            {
                return BadRequest("Booking or Ticket not found");
            }

            // Add the Ticket to the Booking
            _context.BookingsTickets.Add(bookingTicket);
            await _context.SaveChangesAsync();

            return Ok("Ticket added to the Booking");
        }


        // POST: api/Booking/CreateBookingWithTickets
        [HttpPost("CreateBookingWithTickets")]
        public async Task<ActionResult<int>> CreateBookingWithTickets(BookingRequest bookingRequest)
        {
            var booking = new Booking
            {
                UserID = bookingRequest.UserID,
                CreditCardID = bookingRequest.CreditCardID,
                BookingTime = bookingRequest.BookingTime,
                TotalAmount = bookingRequest.TotalAmount
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            if (bookingRequest.TicketIDs != null && bookingRequest.TicketIDs.Any())
            {
                // Create BookingTicket entries to associate tickets with the booking
                foreach (var ticketId in bookingRequest.TicketIDs)
                {
                    var bookingTicket = new BookingTicket
                    {
                        BookingID = booking.BookingID, // Assuming the BookingID is generated upon insertion
                        TicketID = ticketId
                    };
                    _context.BookingsTickets.Add(bookingTicket);
                }

                await _context.SaveChangesAsync();
            }

            return Ok(booking.BookingID);
        }



        // GET: api/Booking/GetAllBookingsWithDetails
        [HttpGet("GetAllBookingsWithDetails")]
        public async Task<ActionResult<List<Booking>>> GetAllBookingsWithDetails()
        {
            // Retrieve all bookings
            var bookings = await _context.Bookings.ToListAsync();

            if (bookings == null || !bookings.Any())
            {
                return NotFound(); // Return a 404 response if no bookings are found
            }

            // Iterate through each booking and manually retrieve related data from different tables
            foreach (var booking in bookings)
            {
                booking.Tickets = await GetTicketsForBooking(booking.BookingID);
                booking.UserAccount = await GetUserAccountForBooking(booking.UserID);
                booking.CreditCard = await GetCreditCardForBooking(booking.CreditCardID);

                // Load related Schedule data for each Ticket
                foreach (var ticket in booking.Tickets)
                {
                    ticket.Schedule = await GetScheduleForTicket(ticket.ScheduleID);
                    ticket.Passenger = await GetPassengerForTicket(ticket.PassengerID);

                    // Load related Bus, BusPath, and Driver data for each Schedule
                    if (ticket.Schedule != null)
                    {
                        ticket.Schedule.Bus = await GetBusForSchedule(ticket.Schedule.BusID);
                        ticket.Schedule.BusPath = await GetBusPathForSchedule(ticket.Schedule.PathID);
                        ticket.Schedule.Driver = await GetDriverForSchedule(ticket.Schedule.DriverID);
                    }
                }
            }

            return Ok(bookings); // Return the list of bookings with related data in the response
        }

      
        // GET: api/Booking/GetBookingWithDetails/{bookingId}
        [HttpGet("GetBookingWithDetails/{bookingId}")]
        public async Task<ActionResult<Booking>> GetBookingWithDetails(int bookingId)
        {
            // Find the booking by ID
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
            {
                return NotFound(); // Return a 404 response if the booking is not found
            }

            // Now, manually retrieve related data from different tables
            booking.Tickets = await GetTicketsForBooking(bookingId);
            booking.UserAccount = await GetUserAccountForBooking(booking.UserID);
            booking.CreditCard = await GetCreditCardForBooking(booking.CreditCardID);


            // Load related Schedule data for each Ticket
            foreach (var ticket in booking.Tickets)
            {
                ticket.Schedule = await GetScheduleForTicket(ticket.ScheduleID);
                ticket.Passenger = await GetPassengerForTicket(ticket.PassengerID);

                // Load related Bus, BusPath, and Driver data for each Schedule
                if (ticket.Schedule != null)
                {
                    ticket.Schedule.Bus = await GetBusForSchedule(ticket.Schedule.BusID);
                    ticket.Schedule.BusPath = await GetBusPathForSchedule(ticket.Schedule.PathID);
                    ticket.Schedule.Driver = await GetDriverForSchedule(ticket.Schedule.DriverID);
                }
            }


            return Ok(booking); // Return the booking with related data in the response
        }


        // GET: api/Booking/GetBookingsByUserId/{userId}
        [HttpGet("GetBookingsByUserId/{userId}")]
        public async Task<ActionResult<List<Booking>>> GetBookingsByUserId(int userId)
        {
            // Find the bookings for a specific user ID
            var bookings = await _context.Bookings
                .Where(b => b.UserID == userId)
                .ToListAsync();

            if (bookings == null || bookings.Count == 0)
            {
                return NotFound(); // Return a 404 response if no bookings are found for the user
            }

            // Now, manually retrieve related data for each booking
            foreach (var booking in bookings)
            {
                booking.Tickets = await GetTicketsForBooking(booking.BookingID);
                booking.CreditCard = await GetCreditCardForBooking(booking.CreditCardID);

                // Load related UserAccount data for each booking
                booking.UserAccount = await GetUserAccountForBooking(booking.UserID);

                // Load related Schedule data for each Ticket
                foreach (var ticket in booking.Tickets)
                {
                    ticket.Schedule = await GetScheduleForTicket(ticket.ScheduleID);
                    ticket.Passenger = await GetPassengerForTicket(ticket.PassengerID);

                    // Load related Bus, BusPath, and Driver data for each Schedule
                    if (ticket.Schedule != null)
                    {
                        ticket.Schedule.Bus = await GetBusForSchedule(ticket.Schedule.BusID);
                        ticket.Schedule.BusPath = await GetBusPathForSchedule(ticket.Schedule.PathID);
                        ticket.Schedule.Driver = await GetDriverForSchedule(ticket.Schedule.DriverID);
                    }
                }
            }

            return Ok(bookings); // Return the list of bookings with related data in the response
        }




        // Helper methods to retrieve related data from different tables
        private async Task<List<Ticket>> GetTicketsForBooking(int bookingId)
        {
            return await _context.BookingsTickets
                .Where(bt => bt.BookingID == bookingId)
                .Select(bt => bt.Ticket)
                .ToListAsync();
        }

        private async Task<UserAccount> GetUserAccountForBooking(int userId)
        {
            return await _context.UserAccounts.FindAsync(userId);
        }
        private async Task<Passenger> GetPassengerForTicket(int passengerId)
        {
            return await _context.Passengers.FindAsync(passengerId);
        }

        

        private async Task<CreditCard> GetCreditCardForBooking(int? creditCardId)
        {
            if (creditCardId.HasValue)
            {
                return await _context.CreditCards.FindAsync(creditCardId);
            }

            return null;
        }


        // Helper method to retrieve related Bus data for a Schedule
        private async Task<Bus> GetBusForSchedule(int busId)
        {
            return await _context.Buses.FindAsync(busId);
        }

        // Helper method to retrieve related BusPath data for a Schedule
        private async Task<BusPath> GetBusPathForSchedule(int pathId)
        {
            return await _context.BusPaths.FindAsync(pathId);
        }

        // Helper method to retrieve related Driver data for a Schedule
        private async Task<Driver> GetDriverForSchedule(int driverId)
        {
            return await _context.Drivers.FindAsync(driverId);
        }

        // Helper method to retrieve related Schedule data for a Ticket
        private async Task<Schedule> GetScheduleForTicket(int scheduleId)
        {
            return await _context.Schedules.FindAsync(scheduleId);
        }

        [HttpDelete("DeletePassengerAndTicket/{ticketId}")]
        public async Task<ActionResult> DeletePassengerAndTicket(int ticketId)
        {
            


            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                 
                    var ticket = await _context.Tickets.FindAsync(ticketId);

                    if (ticket == null)
                    {
                        return NotFound(); // Return a 404 response if the ticket is not found
                    }

                    // Find and delete the associated booking ticket row
                    var bookingTicket = await _context.BookingsTickets
                        .Where(bt => bt.TicketID == ticketId)
                        .FirstOrDefaultAsync();

                    if (bookingTicket != null)
                    {
                        _context.BookingsTickets.Remove(bookingTicket);
                    }

                    var passengerId = ticket.PassengerID;

                    // Delete the passenger associated with the ticket
                    var passenger = await _context.Passengers.FindAsync(passengerId);
                    if (passenger != null)
                    {
                        _context.Passengers.Remove(passenger);
                    }

                    // Delete the ticket itself
                    _context.Tickets.Remove(ticket);

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return Ok(); // Return a success response
                }
                catch (Exception)
                {
                    // Handle exceptions and roll back the transaction if an error occurs
                    transaction.Rollback();
                    return StatusCode(500, "An error occurred while deleting passenger and ticket.");
                }
            }
        }

        // DELETE: api/Booking/DeletePassengerAndTickets/{ticketId}
        [HttpDelete("DeletePassengerAndTickets")]
        public async Task<ActionResult> DeletePassengerAndTickets([FromBody] TicketIdList ticketIds)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var ticketId in ticketIds.TicketIds)
                    {
                        // Find the ticket by ID
                        var ticket = await _context.Tickets.FindAsync(ticketId);

                        if (ticket == null)
                        {
                            // Skip if the ticket is not found, or you can handle it differently
                            continue;
                        }

                        // Find and delete the associated booking ticket row
                        var bookingTicket = await _context.BookingsTickets
                            .Where(bt => bt.TicketID == ticketId)
                            .FirstOrDefaultAsync();

                        if (bookingTicket != null)
                        {
                            _context.BookingsTickets.Remove(bookingTicket);
                        }

                        var passengerId = ticket.PassengerID;

                        // Delete the passenger associated with the ticket
                        var passenger = await _context.Passengers.FindAsync(passengerId);
                        if (passenger != null)
                        {
                            _context.Passengers.Remove(passenger);
                        }

                        // Delete the ticket itself
                        _context.Tickets.Remove(ticket);
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return Ok(); // Return a success response
                }
                catch (Exception)
                {
                    // Handle exceptions and roll back the transaction if an error occurs
                    transaction.Rollback();
                    return StatusCode(500, "An error occurred while deleting passengers and tickets.");
                }
            }
        }

        // DELETE: api/Booking/DeleteBookingWithTickets/{bookingId}
        [HttpDelete("DeleteBookingWithTickets/{bookingId}")]
        public async Task<ActionResult> DeleteBookingWithTickets(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
            {
                return NotFound(); // Return a 404 response if the booking is not found
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                { 

                    // Delete the booking itself
                    _context.Bookings.Remove(booking);

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return Ok(); // Return a success response
                }
                catch (Exception)
                {
                    // Handle exceptions and roll back the transaction if an error occurs
                    transaction.Rollback();
                    return StatusCode(500, "An error occurred while deleting the booking.");
                }
            }
        }



    }
}
