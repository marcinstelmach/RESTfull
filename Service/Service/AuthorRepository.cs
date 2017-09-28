using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DAL;
using Data.DTO;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Service.Helpers;

namespace Service.Service
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryContext _dbContext;
        private readonly IPropertyMappingService _propertyMappingService;

        public AuthorRepository(LibraryContext dbContext, IPropertyMappingService mappingService)
        {
            _dbContext = dbContext;
            _propertyMappingService = mappingService;
        }

        public async Task AddAuthor(Author author)
        {
            if (author != null)
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
            return await _dbContext.Authors.FirstOrDefaultAsync(s => s.Id == authorId);
        }

        public async Task<PageList<Author>> GetAuthors(AuthorResourceParameters authorResourceParameters)
        {
            //var authors = _dbContext.Authors
            //    .OrderBy(s => s.FirstName)
            //    .ThenBy(s => s.LastName).AsQueryable();

            var authors = _dbContext.Authors.ApplySort(authorResourceParameters.OrderBy, _propertyMappingService.GetPropertyMapping<AuthorDto, Author>());

            if (!string.IsNullOrEmpty(authorResourceParameters.Genre))
            {
                var genreForWhereClause = authorResourceParameters.Genre
                    .Trim().ToLowerInvariant();
                authors = authors.Where(s => s.Genre.ToLowerInvariant() == genreForWhereClause);
            }

            if (!string.IsNullOrEmpty(authorResourceParameters.SearchQuery))
            {
                var searchForWhere = authorResourceParameters.SearchQuery.Trim().ToLowerInvariant();
                authors = authors.Where(s =>
                    s.FirstName.Contains(searchForWhere)
                    || s.LastName.Contains(searchForWhere)
                    || s.Genre.Contains(searchForWhere));
            }



            return await PageList<Author>.Create(authors, authorResourceParameters.PageNumber, authorResourceParameters.PageSize);
        }

        public async Task<IEnumerable<Author>> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return await _dbContext.Authors.Where(s => authorIds.Contains(s.Id)).OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() >= 0;
        }

        public async Task UpdateAuthor(Author author)
        {
            var result = await _dbContext.Authors.FirstOrDefaultAsync(s => s.Id == author.Id);
            if (result != null)
            {
                _dbContext.Authors.Update(author);
            }
        }
    }
}
