using TravelLove.helper;
using System.Threading.Tasks;

namespace AttendenceTracker.services
{
    public interface IEmailService
    {
        Task SendEmailAsync(Mailrequest mailrequest);

    }
}
