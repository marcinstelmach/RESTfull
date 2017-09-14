using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DAL;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Service.Service
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryContext _dbContext;

        public AuthorRepository(LibraryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAuthor(Author author)
        {
            if (author!=null)
            {
                _dbContext.Authors.Add(author);
            }
            await Task.CompletedTask;
        }

        public async Task<bool> AuthorExists(Guid authorId)
        {
            return await _dbContext.Authors.AnyAsync(s => s.Id == authorId);
        }

        public async Task DeleteAuthor(Author author)
        {
            _dbContext.Authors.Remove(author);
            await Task.CompletedTask;
        }

        public async Task<Author> GetAuthor(Guid authorId)
        {
            var result = await _dbContext.Authors.FirstOrDefaultAsync(s => s.Id == authorId);
            return result;
        }

        public async Task<IEnumerable<Author>> GetAuthors()
        {
            return await _dbContext.Authors.OrderBy(s => s.FirstName).ThenBy(s => s.LastName).ToListAsync();
        }

        public async Task<IEnumerable<Author>> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return await _dbContext.Authors.Where(s => authorIds.Contains(s.Id)).OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync()>=0;
        }

        public async Task UpdateAuthor(Author author)
        {
            var result = await _dbContext.Authors.FirstOrDefaultAsync(s => s.Id == author.Id);
            if (result!=null)
            {
                _dbContext.Authors.Update(author);
            }
        }
    }
}
