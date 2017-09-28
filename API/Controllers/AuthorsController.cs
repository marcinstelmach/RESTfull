using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.DTO;
using Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service.Helpers;
using Service.Service;

namespace API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;

        public AuthorsController(IAuthorRepository authorRepository, IMapper mapper, IBookRepository bookRepository, IUrlHelper urlHelper, IPropertyMappingService mappingService)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _bookRepository = bookRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = mappingService;
        }

        [HttpGet(Name = "GetAuthors")]
        public async Task<IActionResult> GetAuthors(AuthorResourceParameters authorResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistFor<AuthorDto, Author>(authorResourceParameters.OrderBy))
            {
                return BadRequest();
            }
            var pageListAuthors = await _authorRepository.GetAuthors(authorResourceParameters);
            var previousPageLink = pageListAuthors.HasPrevious
                ? CreateAuthorsResourceUri(authorResourceParameters, ResourceUriType.PreviousPage)
                : null;
            var nextPageLink = pageListAuthors.HasNext
                ? CreateAuthorsResourceUri(authorResourceParameters, ResourceUriType.NextPage)
                : null;

            var paginationMetaData = new
            {
                totalCount = pageListAuthors.TotalCount,
                pageSize = pageListAuthors.PageSize,
                currentPage = pageListAuthors.CurrentPage,
                totalPages = pageListAuthors.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetaData));
            var authorDto = _mapper.Map<IEnumerable<Author>, IEnumerable<AuthorDto>>(pageListAuthors);
            return Ok(authorDto);
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public async Task<IActionResult> GetAuthor(Guid authorId)
        {
            var author = await _authorRepository.GetAuthor(authorId);
            if (author == null)
            {
                return NotFound("Author not found");
            }
            var authorDto = _mapper.Map<Author, AuthorDto>(author);
            return Ok(authorDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorForCreation authorForCreation)
        {
            if (!ModelState.IsValid) return BadRequest();
            var author = _mapper.Map<AuthorForCreation, Author>(authorForCreation);
            await _authorRepository.AddAuthor(author);
            if (!await _authorRepository.SaveAsync())
            {
                return StatusCode(500, "A problem with save");
            }

            var authorDto = _mapper.Map<Author, AuthorDto>(author);
            return CreatedAtRoute("GetAuthor", new { authorId = authorDto.Id }, authorDto);

        }

        //Method to provide that someone can't send POST to author ID
        [HttpPost("{authorId}")]
        public async Task<IActionResult> BlockAuthorCreation(Guid authorId)
        {
            if (await _authorRepository.AuthorExists(authorId))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("{authorId}")]
        public async Task<IActionResult> DeleteAuthor(Guid authorId)
        {
            var author = await _authorRepository.GetAuthor(authorId);
            if (author == null)
            {
                return NotFound();
            }

            var authorBooks = await _bookRepository.GetBooksForAuthor(authorId);
            if (authorBooks.Any())
            {
                authorBooks.ToList().ForEach(async book => await _bookRepository.DeleteBook(book));
            }
            await _authorRepository.DeleteAuthor(author);
            await _authorRepository.SaveAsync();
            return NoContent();
        }

        private string CreateAuthorsResourceUri(AuthorResourceParameters authorResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            orderBy = authorResourceParameters.OrderBy,
                            searchQuery = authorResourceParameters.SearchQuery,
                            genre = authorResourceParameters.Genre,
                            pageNumber = authorResourceParameters.PageNumber - 1,
                            pageSize = authorResourceParameters.PageSize

                        });
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            orderBy = authorResourceParameters.OrderBy,
                            searchQuery = authorResourceParameters.SearchQuery,
                            genre = authorResourceParameters.Genre,
                            pageNumber = authorResourceParameters.PageNumber + 1,
                            pageSize = authorResourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            orderBy = authorResourceParameters.OrderBy,
                            searchQuery = authorResourceParameters.SearchQuery,
                            genre = authorResourceParameters.Genre,
                            pageNumber = authorResourceParameters.PageNumber,
                            pageSize = authorResourceParameters.PageSize
                        });
            }
        }
    }
}