
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementPro.Data;
using TaskManagementPro.DTOs;
using TaskManagementPro.Interfaces;
using TaskManagementPro.Models;

namespace TaskManagementPro.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthController(
            ApplicationDbContext context,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _context.Users
                .AnyAsync(x => x.Email == email))
            {
                return Conflict(new
                {
                    message = "Email already registered"
                });
            }

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt
                    .HashPassword(dto.Password),
                RoleId = 2
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(
                    dto.Password, user.PasswordHash))
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            var accessToken =
                _tokenService.GenerateAccessToken(user);

            var refreshToken =
                _tokenService.GenerateRefreshToken();

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = _tokenService.HashRefreshToken(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(
                    int.Parse(_configuration[
                        "Jwt:RefreshTokenExpiryDays"]!))
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken,
                refreshToken,
                role = user.Role.Name
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            RefreshTokenDTO dto)
        {
            var tokenHash = _tokenService
                .HashRefreshToken(dto.RefreshToken);

            var storedToken = await _context.RefreshTokens
                .Include(x => x.User)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Token == tokenHash);

            if (storedToken == null ||
                storedToken.IsRevoked ||
                storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized(new
                {
                    message = "Invalid or expired refresh token"
                });
            }

            storedToken.IsRevoked = true;

            var newRefreshToken =
                _tokenService.GenerateRefreshToken();

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = storedToken.UserId,
                Token = _tokenService
                    .HashRefreshToken(newRefreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(
                    int.Parse(_configuration[
                        "Jwt:RefreshTokenExpiryDays"]!))
            });

            var accessToken =
                _tokenService.GenerateAccessToken(
                    storedToken.User);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken,
                refreshToken = newRefreshToken
            });
        }
    }
}
