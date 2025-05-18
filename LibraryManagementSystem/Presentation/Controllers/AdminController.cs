using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Presentation.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            var userRole = HttpContext.Items["UserRole"] as string;

            if (userRole != "Admin")  // 🔹 Restrict Access
                return Unauthorized(new { message = "Access Denied: Admins Only" });

            return Ok(new { message = "Welcome to the Admin Dashboard" });
        }
    }
}
