using LibraryManagementSystem.Entities;
using LibraryManagementSystem.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Presentation.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BooksController : ControllerBase // BooksController is an API controller for books.
    {
        private readonly LibraryService _libraryService;

        public BooksController(LibraryService libraryService) 
        {
            _libraryService = libraryService;
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddBook([FromBody] AddBookRequest request)
        {
            try
            {
                _libraryService.AddBook(request.Title, request.Author );
                return Ok(new { message = $"Book '{request.Title}' added Successfully " });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("borrow/{title}")] //POST /api/books/borrow/{title} → Calls BorrowBook(title).
        [Authorize(Roles = "User,Admin")]
        public IActionResult BorrowBook(string title) 
        {
            try
            {
                _libraryService.BorrowBook(title);
                return Ok(new { message = $"you have borrowed '{title}'" }); // Returns success or error messages in JSON format
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message }); // Returns success or error messages in JSON format
            }
        }
    }

        // DTO for AddBook request
    public class AddBookRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
    }
}
