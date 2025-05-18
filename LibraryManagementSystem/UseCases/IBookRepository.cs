using LibraryManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.UseCases
{
    public interface IBookRepository
    {
        Book GetBookByTitle(string title);
        void UpdateBook(Book book);
        void AddBook(Book book);
    }
}
