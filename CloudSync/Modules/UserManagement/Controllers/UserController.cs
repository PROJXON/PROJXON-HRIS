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
    public class UserController(DatabaseContext context) : ControllerBase
    {
        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var userList = await context.Users.ToListAsync();
            List<UserDTO> userDtoList = [];
            
            userDtoList.AddRange(userList.Select(user => new UserDTO
            {
                Id = user.Id, Email = user.Email, CreateDateTime = user.CreateDateTime, LastLoginDateTime = user.LastLoginDateTime,
            }));

            return userDtoList;
        }
        
        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await context.Users.FindAsync(id);
        
            if (user == null)
            {
                return NotFound();
            }
            
            return new UserDTO()
            {
                Id = user.Id,
                Email = user.Email,
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
        
            context.Entry(user).State = EntityState.Modified;
        
            try
            {
                await context.SaveChangesAsync();
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
            
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var createUserDTO = new CreateUserDTO
            {
                Id = user.Id,
                Password = passwordAndHash.GeneratedPassword,
                Email = user.Email,
                CreateDateTime = user.Email,
                LastLoginDateTime = user.LastLoginDateTime,
                UserSettings = user.UserSettings ?? null
            };

            return CreatedAtAction("GetUser", new { id = user.Id }, createUserDTO);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> UserExists(int id)
        {
            return await context.Users.AnyAsync(e => e.Id == id);
        }
    }
}
