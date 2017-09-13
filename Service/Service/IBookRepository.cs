using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data.Model;

namespace Service.Service
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetBooksForAuthor(Guid authorId);
        Task<Book> GetBookForAuthor(Guid authorId, Guid bookId);
        Task AddBookForAuthor(Guid authorId, Book book);
        Task UpdateBookForAuthor(Book book);
        Task DeleteBook(Book book);
        Task<bool> SaveAsync();
    }
}