using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data.Model;

namespace Service.Service
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAuthors();
        Task<Author> GetAuthor(Guid authorId);
        Task<IEnumerable<Author>> GetAuthors(IEnumerable<Guid> authorIds);
        Task AddAuthor(Author author);
        Task DeleteAuthor(Author author);
        Task UpdateAuthor(Author author);
        Task<bool> AuthorExists(Guid authorId);
        Task<bool> SaveAsync();
    }
}
