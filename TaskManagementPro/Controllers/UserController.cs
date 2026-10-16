
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementPro.Data;
using TaskManagementPro.DTOs;
using TaskManagementPro.Models;

namespace TaskManagementPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(
            [FromBody] CreateUserDTO dto)
        {
            if (await _context.Users.AnyAsync(
                x => x.Email == dto.Email))
            {
                return BadRequest("Email already exists.");
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.Id == dto.RoleId);

            if (role == null)
            {
                return BadRequest("Invalid RoleId.");
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User created successfully.",
                userId = user.Id
            });
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.RoleId,
                    Role = x.Role.Name
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}

