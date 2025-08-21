using Microsoft.EntityFrameworkCore;
using MyApi.Models;  

namespace MyApi   
{
    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
