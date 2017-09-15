using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Data.DTO;
using Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Service;

namespace API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, IMapper mapper, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _authorRepository = authorRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetBooksForAuthor(Guid authorId)
        {
            if (!await _authorRepository.AuthorExists(authorId))
                return NotFound();
            var books = await _bookRepository.GetBooksForAuthor(authorId);

            var mappedBooks = _mapper.Map<IEnumerable<Book>, IEnumerable<BookDto>>(books);
            return Ok(mappedBooks);
        }

        [HttpGet("{bookId}", Name = "GetBookForAuthor")]
        public async Task<IActionResult> GetBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!await _authorRepository.AuthorExists(authorId))
                return NotFound();


            var book = await _bookRepository.GetBookForAuthor(authorId, bookId);
            if (book == null)
                return NotFound();

            var mappedBook = _mapper.Map<Book, BookDto>(book);
            return Ok(mappedBook);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookForAuthor(Guid authorId, [FromBody] BookForCreation bookForCreation)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (!await _authorRepository.AuthorExists(authorId))
                return NotFound();

            var book = _mapper.Map<BookForCreation, Book>(bookForCreation);
            await _bookRepository.AddBookForAuthor(authorId, book);
            if (!await _bookRepository.SaveAsync())
                return StatusCode(500, "Fault with saving data");
            var bookDto = _mapper.Map<Book, BookDto>(book);
            return CreatedAtRoute("GetBookForAuthor", new {bookId = book.Id, authorId = book.AuthorId}, bookDto);
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!await _authorRepository.AuthorExists(authorId))
                return NotFound();
            var bookToDelete = await _bookRepository.GetBookForAuthor(authorId, bookId);
            if (bookToDelete == null)
                return NotFound();

            await _bookRepository.DeleteBook(bookToDelete);
            if (!await _bookRepository.SaveAsync())
                throw new Exception($"Deleting book {bookId} for author {authorId} failed on save");

            return NoContent();
        }

        [HttpPut("{bookId}")]
        public async Task<IActionResult> UpdateBook(Guid authorId, Guid bookId,
            [FromBody] BookForUpdateDto bookForUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (!await _authorRepository.AuthorExists(authorId))
                return NotFound();

            var book = await _bookRepository.GetBookForAuthor(authorId, bookId);
            if (book == null)
            {
                var bookToAdd = _mapper.Map<BookForUpdateDto, Book>(bookForUpdateDto);
                bookToAdd.Id = bookId;
                bookToAdd.AuthorId = authorId;


                await _bookRepository.AddBookForAuthor(authorId, bookToAdd);
                if (!await _bookRepository.SaveAsync())
                    return StatusCode(StatusCodes.Status500InternalServerError, "Fault while saving data");

                return CreatedAtRoute("GetBookForAuthor", new {authorId = bookToAdd.AuthorId, bookId = bookToAdd.Id},
                    bookToAdd);
            }

            _mapper.Map(bookForUpdateDto, book); //this mapping updates book, and next save
            if (!await _bookRepository.SaveAsync())
                throw new Exception("Fault while saving database after update");
            var bookDto = _mapper.Map<Book, BookDto>(book);

            return Ok(bookDto);
        }
    }
}