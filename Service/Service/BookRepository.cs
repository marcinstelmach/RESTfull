using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.DAL;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Service.Service
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _dbContext;

        public BookRepository(LibraryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Book>> GetBooksForAuthor(Guid authorId)
        {
            return await _dbContext.Books.Where(s => s.AuthorId == authorId).ToListAsync();
        }

        public async Task<Book> GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(s => s.AuthorId == authorId && s.Id == bookId);
        }

        public async Task AddBookForAuthor(Guid authorId, Book book)
        {
            var author = await _dbContext.Authors.FirstOrDefaultAsync(s => s.Id == authorId);
            if (author == null)
                book.Id = Guid.NewGuid();
            author.Books.Add(book);
        }

        public async Task UpdateBookForAuthor(Book book)
        {
            _dbContext.Books.Update(book);
            await Task.CompletedTask;
        }

        public async Task DeleteBook(Book book)
        {
            _dbContext.Books.Remove(book);
            await Task.CompletedTask;
        }

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() >= 0;
        }
    }
}