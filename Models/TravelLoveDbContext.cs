using Microsoft.EntityFrameworkCore;
using TravelLove.Models;

namespace TravelLove
{
    public class TravelLoveDbContext : DbContext
    {

        public TravelLoveDbContext(DbContextOptions<TravelLoveDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        
        public virtual DbSet<Bus> Buses { get; set; }
        
        public virtual DbSet<Driver> Drivers { get; set; }
        
        public virtual DbSet<BusPath> BusPaths { get; set; }
        
        public virtual DbSet<Schedule> Schedules { get; set; }
        
        public virtual DbSet<Seat> Seats { get; set; }
        
        public virtual DbSet<Ticket> Tickets { get; set; }
        
        public virtual DbSet<CreditCard> CreditCards { get; set; }
        
        public virtual DbSet<Review> Reviews { get; set; }
        
        public virtual DbSet<Passenger> Passengers { get; set; }
        
        public virtual DbSet<BusSeat> BusSeats { get; set; }    
        
        public virtual DbSet<Booking> Bookings { get; set; }

        public virtual DbSet<BookingTicket> BookingsTickets { get; set; }    

    }



}
