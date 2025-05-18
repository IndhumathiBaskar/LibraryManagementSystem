using System;


namespace LibraryManagementSystem.Entities
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public bool IsAvailable { get; private set; } = true;

        public Book(string title, string author)
        {
            Title = title;
            Author = author;
            IsAvailable = true;
        }

        public void Borrow()
        {
            if (!IsAvailable) 
                throw new InvalidOperationException("Book is already Borrowed.");

            IsAvailable = false;
        }

        public void Return()
        {
            if (IsAvailable)
                throw new InvalidOperationException("Book is already returned.");

            IsAvailable = true;
        }

    }
}
