using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudSync.Infrastructure;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Services;
using Shared.DTOs.User;

namespace CloudSync.Modules.UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public UserController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var userList = await _context.Users.ToListAsync();
            List<UserDTO> userDtoList = [];
            
            userDtoList.AddRange(userList.Select(user => new UserDTO
            {
                Id = user.Id, Username = user.Username, CreateDateTime = user.CreateDateTime, LastLoginDateTime = user.LastLoginDateTime,
            }));

            return userDtoList;
        }
        
        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
        
            if (user == null)
            {
                return NotFound();
            }
            
            return new UserDTO()
            {
                Id = user.Id,
                Username = user.Username,
                CreateDateTime = user.CreateDateTime,
                LastLoginDateTime = user.LastLoginDateTime,
                UserSettings = user.UserSettings ?? null
            };
        }
        
        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
        
            _context.Entry(user).State = EntityState.Modified;
        
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (UserExists(id).IsCanceled || UserExists(id).IsFaulted)
                {
                    return NotFound();
                }
            }
        
            return NoContent();
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(User user)
        {
            var passwordAndHash = PasswordService.GeneratePasswordAndHash();
            
            user.Password = passwordAndHash.HashedPassword;
            Console.WriteLine(user.Password);
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var createUserDTO = new CreateUserDTO
            {
                Id = user.Id,
                Password = passwordAndHash.GeneratedPassword,
                Username = user.Username,
                CreateDateTime = user.CreateDateTime,
                LastLoginDateTime = user.LastLoginDateTime,
                UserSettings = user.UserSettings ?? null
            };

            return CreatedAtAction("GetUser", new { id = user.Id }, createUserDTO);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
}
