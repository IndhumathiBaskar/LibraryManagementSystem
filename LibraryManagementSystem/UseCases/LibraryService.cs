using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Entities;

namespace LibraryManagementSystem.UseCases
{
    public class LibraryService
    {
        private readonly IBookRepository _bookRepository;

        public LibraryService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public void AddBook(string title, string author)
        {
            var existingBook  = _bookRepository.GetBookByTitle(title);

            if (existingBook != null)
            {
                throw new Exception("Book already exists.");
            }

            var newBook = new Book(title, author);

            _bookRepository.AddBook(newBook);
        }
        public void BorrowBook(string title)
        {
            var book = _bookRepository.GetBookByTitle(title);

            if (book == null) throw new Exception("Book Not Found");

            book.Borrow();

            _bookRepository.UpdateBook(book);

        }

        public void ReturnBook (string title)
        {
            var book = _bookRepository.GetBookByTitle(title);

            if (book == null) throw new Exception("Book Not Found");

            book.Return();

            _bookRepository.UpdateBook(book);
        }
    }
}
