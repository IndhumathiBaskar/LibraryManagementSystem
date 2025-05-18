using LibraryManagementSystem.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace LibraryManagementSystem.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtService _jwtService;
        public AuthController(AuthService authService,JwtService jwtService) 
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        // 🔹 User Registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.Register(request.Username, request.Password, request.Role);

            if (result == "Username already exists")
            {
                return BadRequest(new { message = result });
            }
            return Ok(new { message = result });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            var (token, refreshToken) = await _authService.Login(request.Username, request.Password, _jwtService);

            if (token == null) return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { token, refreshToken });

        }

        // 🔹 Refresh Token Endpoint
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var (newToken, newRefreshToken) = await _authService.RefreshToken(request.RefreshToken, _jwtService);

            if (newToken == null) return Unauthorized(new { message = "Invalid or expired refresh token" });

            return Ok(new { token = newToken, refreshToken = newRefreshToken });
        }

    }
}


// 🔹 Request Models
public class RegisterRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}