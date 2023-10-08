using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Text;
using System.Diagnostics;
using AttendenceTracker.services;
using TravelLove.helper;
using TravelLove.DTO.Requests;
using NHibernate.Mapping;
using System.Drawing;

namespace TravelLove.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

            private readonly IEmailService emailService;
            public CustomerController(IEmailService service)
            {
                this.emailService = service;
            }


            [HttpPost("SendMail")]
            public async Task<IActionResult> SendMail()
            {
                try
                {
                    Mailrequest mailrequest = new Mailrequest();
                    mailrequest.ToEmail = "siva02208@gmail.com";
                    mailrequest.Subject = "Absence Notification for";
                    mailrequest.Body = "Thanks";
                    await emailService.SendEmailAsync(mailrequest);
                    return Ok();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }






        [HttpPost("SendEmailToMultiple")]
        public async Task<IActionResult> SendEmailToMultiple([FromBody] BookingEmailRequest bookingEmailRequest)
        {
            try
            {
                if (bookingEmailRequest.BookingDataList == null || bookingEmailRequest.EmailIds == null || bookingEmailRequest.BookingDataList.Count == 0 || bookingEmailRequest.EmailIds.Count == 0)
                {
                    return BadRequest("Invalid input data");
                }

                var lastPersonEmail = bookingEmailRequest.EmailIds.Last();
                var otherEmails = bookingEmailRequest.EmailIds.Where(email => email != lastPersonEmail).ToList();

                foreach (var bookingData in bookingEmailRequest.BookingDataList)
                {
                    foreach (var emailId in otherEmails)
                    {
                        Mailrequest mailrequest = new Mailrequest();
                        mailrequest.ToEmail = emailId;
                        mailrequest.Subject = $"Ticket Information for {bookingData.PathName}";
                        mailrequest.Body = GetTicketHtml(bookingData);

                        await emailService.SendEmailAsync(mailrequest);
                    }
                }

                // Send a combined email to the last person
                var combinedHtml = string.Join("<hr/>", bookingEmailRequest.BookingDataList.Select(GetTicketHtml));
                Mailrequest combinedMail = new Mailrequest();
                combinedMail.ToEmail = lastPersonEmail;
                combinedMail.Subject = "Combined Ticket Information";
                combinedMail.Body = combinedHtml;
                await emailService.SendEmailAsync(combinedMail);

                return Ok(new { message = "Emails sent successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Helper method to generate HTML ticket
        private string GetTicketHtml(BookingData bookingData)
        {
            // You can construct the HTML ticket here based on the provided structure
            // and data from bookingData
            return $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css'>
                    <style>
                         /* Custom styles for the ticket */
      .ticket {{
        width: 650px;
        border: 2px solid #333;
        border-radius: 10px;
        background-color: #fff;
        font-family: Arial, sans-serif;
        margin: 20px auto;
        box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.2);
      }}

      /* Header styles */
      .header {{
        background-color: #333;
        color: white;
        padding: 10px;
        display: flex;
        justify-content: space-between;
        align-items: center;
        border-top-left-radius: 10px;
        border-top-right-radius: 10px;
      }}

      /* Logo styles */
      .logo {{
        font-size: 20px;
        font-weight: bold;
      }}

      /* Bus and passenger details container */
      .details-container {{
        display: flex;
      }}

      /* Styles for bus details */
      .bus-details {{
        flex: 1;
        padding: 10px;
      }}

      /* Styles for passenger details */
      .passenger-details {{
        flex: 1;
        padding: 10px;
      }}

      /* Styles for rows within bus and passenger details */
      .row {{
        display: flex;
        justify-content: space-between;
        padding: 5px 0;
      }}

      /* Styles for path and amount section */
      .path-amount {{
        background-color: #333;
        color: white;
        padding: 10px;
        border-bottom-left-radius: 10px;
        border-bottom-right-radius: 10px;
      }}

      /* Styles for path and amount section rows */
      .path-amount .row {{
        display: flex;
        justify-content: space-between;
        padding: 5px 0;
      }}

      /* Add custom styles for specific elements if needed */

      body {{
        padding: 0;
        margin: 0;
        width: 100%;
        height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        font-family: sans-serif;
        font-size: 12px;
        font-family: ""Poppins"", sans-serif;
        background-color: #b7b5e4;
      }}

      /* Main Ticket Style */
      .ticketContainer {{
        display: flex;
        flex-direction: column;
        align-items: center;
      }}

      .ticket {{
        animation: bouncingCard 0.6s ease-out infinite alternate;
        background-color: white;
        color: darkslategray;
        border-radius: 12px;
      }}

      /* Ticket Content */
      .ticketTitle {{
        font-size: 1.5rem;
        font-weight: 700;
        padding: 12px 16px 4px;
      }}

      hr {{
        width: 90%;
        border: 1px solid #efefef;
      }}

      .ticketDetail {{
        font-size: 1.1rem;
        font-weight: 500;
        padding: 4px 16px;
      }}

      .ticketSubDetail {{
        display: flex;
        justify-content: space-between;
        font-size: 1rem;
        padding: 12px 16px;
      }}

      .ticketSubDetail .code {{
        margin-right: 24px;
      }}

      /* Ticket Ripper */
      .ticketRip {{
        display: flex;
        justify-content: space-between;
        align-items: center;
      }}

      .circleLeft {{
        width: 12px;
        height: 24px;
        background-color: #b7b5e4;
        border-radius: 0 12px 12px 0;
      }}

      .ripLine {{
        width: 100%;
        border-top: 3px solid #b7b5e4;
        border-top-style: dashed;
      }}

      .circleRight {{
        width: 12px;
        height: 24px;
        background-color: #b7b5e4;
        border-radius: 12px 0 0 12px;
      }}

      /* Additional styles for the second ticket container */
      .secondTicketContainer {{
        margin-top: 40px;
      }}

      .secondTicket .header {{
        background-color: #0069d9;
      }}

      .secondTicket .logo {{
        font-size: 18px;
        font-weight: bold;
      }}

      .secondTicket .bus-details {{
        padding: 8px;
      }}

      .secondTicket .bus-details .row {{
        justify-content: flex-end;
      }}

      .secondTicket .card {{
        width: 550px;
      }}

      .secondTicket .ticketDetail .row {{
        padding: 8px 0;
      }}

      .secondTicket .ticketSubDetail {{
        padding: 8px 16px;
      }}
                    </style>
                </head>
                <body>
<div class=""text-center"">


 <div class=""container-header"">
        <h1 style=""margin:5px;"">Ride To Home</h1>
        <p class=""confirmation-message"" style=""margin:5px;"">Sucessfully Booked</p>
	<div class=""centered-image"">
            <img src=""https://cdn3.iconfinder.com/data/icons/social-messaging-ui-color-line/254000/39-512.png"" alt=""Confirmation Image"" width=""200"" style=""width:120px;"">
        </div>
        <p class=""confirmation-message"">Thankyou for choosing us!</p>


                    <div class='container mt-5'>
                        <div class='row'>
                            <div class='col-md-6 offset-md-3'>
                                <!-- Ticket Container -->
                                 <div class=""ticketContainer"">
                <div class=""ticket"">
                  <div class=""ticketTitle"">
                    <div class=""header bg-dark text-white"">
                      <div class=""logo font-weight-bold"">RideToHome</div>
                      <div class=""bus-details"">
                        <div class=""row"">
                          <div class=""col"">
                            <div class=""row"" style=""justify-content: flex-end"">
                             { bookingData.PathName }
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <hr />
                  <div class=""ticketDetail row"">
                    <div class=""col-md-7"">
                      <div class=""bus-details"">
                        <div class=""row"">
                          <div class=""col"">
                           { bookingData.StartingPoint }
                          </div>

                          <div class=""col"">
                           {
                                bookingData.ArrivalTime
                              
                            }
                          </div>
                        </div>
                        <div class=""row"">
                          <div class=""col"">
                           { bookingData.DestinationPoint }
                          </div>

                          <div class=""col"">
                           {
                              
                                bookingData.DepartureTime
                              
                            }
                          </div>
                        </div>
                        <div class=""row"">
                          <div class=""col"">
                           { bookingData.BusLicensePlate }
                          </div>

                          <div class=""col"">
                           { bookingData.Modal } ({ bookingData.Type } )
                          </div>
                          <div class=""col"">
                           { bookingData.BookingTime }
                          </div>
                        </div>
                      </div>
                    </div>
                    <div class=""col-md-5"">
                      <div class=""passenger-details"">
                        <div class=""row"">
                          <div class=""col"">{ bookingData.SeatNumber }</div>
                        </div>
                        <div class=""row"">
                          <div class=""col"">
                           { bookingData.Name }
                          </div>
                        </div>
                        <div class=""row"">
                          <div class=""col"">{ bookingData.Email }</div>
                        </div>
                        <div class=""row"">
                          <div class=""col"">{ bookingData.PhoneNumber }</div>
                        </div>
                        <div class=""row"">
                          <div class=""col"">
                           { bookingData.Age }
                           { bookingData.Gender }
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class=""ticketRip"">
                    <div class=""circleLeft""></div>
                    <div class=""ripLine""></div>
                    <div class=""circleRight""></div>
                  </div>
                  <div class=""ticketSubDetail"">
                    <div class=""code"">
                      Ticket ID:
                     { bookingData.TicketID }
                    </div>
                    <div class=""date"">INR:{ bookingData.SeatAmount }</div>
                  </div>
                </div>
                <div class=""ticketShadow""></div>
              </div>
                            </div>
                        </div>
                    </div>





















<div class=""why-choose-us"">
            <h2>Why Choose Us</h2>
            <p>We understand that choosing the right ticket booking service is important. Here's why you should choose us:</p>
            <ul>
                <li><strong>Convenience:</strong> Our platform is user-friendly, making it easy for you to book tickets anytime, anywhere.</li>
                <li><strong>Reliability:</strong> We have a track record of providing reliable ticketing services, ensuring you have a smooth experience.</li>
                <li><strong>Selection:</strong> We offer a wide range of destinations and ticket options to suit your travel needs.</li>
                <li><strong>Competitive Pricing:</strong> Our prices are competitive, and we offer various discounts and promotions to save you money.</li>
                <li><strong>Customer Support:</strong> Our dedicated customer support team is available to assist you with any questions or concerns.</li>
            </ul>
            <p>Thank you for choosing us for your ticket booking needs. We look forward to serving you again in the future.</p>
        </div>
    </div>
</div>
                </body>
                </html>
            ";
        }



    }

}
