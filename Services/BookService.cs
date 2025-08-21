using MyApi;     // for BookDbContext
using MyApi.Models;      // for Book model
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApi.Services
{
    public class BookService : IBookService
    {
        private readonly BooksDbContext _context;
         private readonly ILogger<BookService> _logger;

        public BookService(BooksDbContext context,ILogger<BookService> logger)
        {
            _context = context; // DB context is injected here
            _logger = logger;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            var books = await _context.Books.ToListAsync();
            _logger.LogInformation("Books fetched: {Count}", books.Count);
            return books;
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _logger.LogInformation("Book fetched: {Title} by {Author}", book.Title, book.Author);
            }
            return book;
            
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Book created: {Title} by {Author}", book.Title, book.Author);
            return book;
        }

        public async Task UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                _logger.LogWarning("Book deleted with ID {BookId}", id);
            }
        }
    }
}
