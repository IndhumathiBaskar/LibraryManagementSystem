using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Infrastructure.Data;
using LibraryManagementSystem.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryManagementSystem.Infrastructure.Repositories
{
    public class SqlBookRepository : IBookRepository
    {

        private readonly AppDbContext _context; // Uses AppDbContext to interact with SQL Server.

        public SqlBookRepository(AppDbContext context) 
        {
            _context = context;
        }

        public void AddBook(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();

        }
        public Book GetBookByTitle(string title) // GetBookByTitle() fetches a book from the database.
        {
            return _context.Books.FirstOrDefault(b => b.Title == title);
        }

        public void UpdateBook(Book book) // UpdateBook() updates the book’s availability status.
        {
            _context.Books.Update(book);
            _context.SaveChanges();
        }
    }
}
