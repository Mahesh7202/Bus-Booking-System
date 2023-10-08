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
    public class UserAccountController : ControllerBase
    {
        private readonly TravelLoveDbContext _context;

        public UserAccountController(TravelLoveDbContext context)
        {
            _context = context;
        }

        // GET: api/UserAccount
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAccount>>> GetUserAccounts()
        {
            return await _context.UserAccounts.ToListAsync();
        }

        // GET: api/UserAccount/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAccount>> GetUserAccount(int id)
        {
            var userAccount = await _context.UserAccounts.FindAsync(id);

            if (userAccount == null)
            {
                return NotFound();
            }

            return userAccount;
        }

        // PUT: api/UserAccount/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAccount(int id, UserAccount userAccount)
        {
            if (id != userAccount.UserID)
            {
                return BadRequest();
            }

            _context.Entry(userAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAccountExists(id))
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

        // POST: api/UserAccount
        [HttpPost]
        public async Task<ActionResult<UserAccount>> PostUserAccount(UserAccount userAccount)
        {
            _context.UserAccounts.Add(userAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAccount", new { id = userAccount.UserID }, userAccount);
        }

        // DELETE: api/UserAccount/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAccount(int id)
        {
            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }

            _context.UserAccounts.Remove(userAccount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/UserAccount/Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserAccount>> LoginUser(LoginModel loginModel)
        {
            UserAccount userAccount = null;

            // Check if the request contains an email or mobile number and password
            if (!string.IsNullOrWhiteSpace(loginModel.EmailOrMobile) && !string.IsNullOrWhiteSpace(loginModel.Password))
            {
                // Try to find a user with the given email or mobile number
                userAccount = await _context.UserAccounts
                    .FirstOrDefaultAsync(u =>
                        (u.Email == loginModel.EmailOrMobile || u.PhoneNumber == loginModel.EmailOrMobile) &&
                        u.Password == loginModel.Password);

                if (userAccount == null)
                {
                    // If no user is found with the provided credentials, return Unauthorized
                    return Unauthorized();
                }
            }
            else
            {
                // If either email/mobile or password is missing in the request, return BadRequest
                return BadRequest("Email/Mobile and Password are required.");
            }

            // Return the user account if login is successful
            return userAccount;
        }



        private bool UserAccountExists(int id)
        {
            return _context.UserAccounts.Any(e => e.UserID == id);
        }
    }

  
}
