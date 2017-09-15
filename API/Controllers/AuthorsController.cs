using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.DTO;
using Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Service;

namespace API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, IMapper mapper, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {

            //throw new Exception("Just for test");

            var result = await _authorRepository.GetAuthors();
            var authorDto = _mapper.Map<IEnumerable<Author>, IEnumerable<AuthorDto>>(result);
            return Ok(authorDto);
        }

        [HttpGet("{authorId}", Name = "Get_Author")]
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
            if (! await _authorRepository.SaveAsync())
            {
                return StatusCode(500, "A problem with save");
            }

            var authorDto = _mapper.Map<Author, AuthorDto>(author);
            return CreatedAtRoute("Get_Author", new {authorId = authorDto.Id}, authorDto);

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
            if (author==null)
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
    }
}