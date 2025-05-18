using LibraryManagementSystem.Infrastructure.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using LibraryManagementSystem.Entities;
using System.Security.Cryptography;
using System;

namespace LibraryManagementSystem.UseCases
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        //Register() → Hashes password and stores the user in the database.
        public async Task<string> Register(string username, string password, string role)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == username))
                return "Username already exists";

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password); // hash Password

            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(1);

            var user = new User { 
                UserName = username, 
                PasswordHash = passwordHash , 
                Role = role,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshTokenExpiry
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully";
        }

        //Login() → Verifies password and returns a JWT token if valid.
        public async Task<(string token, string refreshToken)> Login(string username,string password, JwtService jwtService)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (null,null);

            var token = jwtService.GenerateToken(username, user.Role);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);

            await _context.SaveChangesAsync();

            return (token, refreshToken);

        }

        // 🔹 Validate & Refresh Access Token
        public async Task<(string token, string refreshToken)> RefreshToken(string oldRefreshToken, JwtService jwtService)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == oldRefreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return (null, null);  // 🔹 Invalid or Expired Token

            var newAccessToken = jwtService.GenerateToken(user.UserName, user.Role);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();
            return (newAccessToken, newRefreshToken);
        }
    }
}
