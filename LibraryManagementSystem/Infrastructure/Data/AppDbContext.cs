using LibraryManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext // DbContext connects to SQL Server.
    {
        public DbSet<Book> Books { get; set; } // DbSet<Book> represents the Books table in the database.

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public AppDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // The OnModelCreating method ensures Title is used as a Primary Key.
        {
            modelBuilder.Entity<Book>().HasKey(b => b.Id); // Using Id as a Primary Key

            modelBuilder.Entity<Book>().Property(b => b.Id)
                                       .ValueGeneratedOnAdd(); // Make Id auto-incrementing
        }
    }
}
