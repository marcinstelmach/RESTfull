using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Data.DTO;
using Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Helpers;
using Service.Service;

namespace API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookRepository bookRepository, IMapper mapper, IAuthorRepository authorRepository, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _authorRepository = authorRepository;
            _logger = logger;
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
            if (bookForCreation == null)
                return BadRequest();

            if (bookForCreation.Title==bookForCreation.Description)
            {
                ModelState.AddModelError(nameof(BookForCreation),
                    "The provided Description should be different from the title.");
            }


            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectRestult(ModelState);
            }

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

            _logger.LogInformation(100, $"Book {bookId} for author {authorId} was deleted");

            return NoContent();
        }

        [HttpPut("{bookId}")]
        public async Task<IActionResult> UpdateBook(Guid authorId, Guid bookId,
            [FromBody] BookForUpdateDto bookForUpdateDto)
        {
            if (bookForUpdateDto == null)
                return BadRequest();

            if (bookForUpdateDto.Description == bookForUpdateDto.Title)
            {
                ModelState.AddModelError(nameof(BookForUpdateDto), "The provided description shouldn't be equal to title");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectRestult(ModelState);    
            }

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

        [HttpPatch("{bookId}")]
        public async Task<IActionResult> PartiallyUpdateBookForAuthor(Guid authorId, Guid bookId,
            [FromBody] JsonPatchDocument<BookForUpdateDto> jsonPatchDocument)
        {
            if (jsonPatchDocument == null)
                return BadRequest();

            var author = await _authorRepository.GetAuthor(authorId);
            if (author == null)
                return NotFound();

            var bookFromRepo = await _bookRepository.GetBookForAuthor(authorId, bookId);
            if (bookFromRepo == null)
            {
                var bookUpdateDto = new BookForUpdateDto();
                jsonPatchDocument.ApplyTo(bookUpdateDto, ModelState);
                if (bookUpdateDto.Title == bookUpdateDto.Description)
                {
                    ModelState.AddModelError(nameof(BookForUpdateDto), "The provided description shoud be different from the title");
                }

                await TryUpdateModelAsync(bookUpdateDto);
                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectRestult(ModelState);
                }

                var bookToAdd = _mapper.Map<Book>(bookUpdateDto);
                bookToAdd.Id = bookId;
                bookToAdd.AuthorId = authorId;

                await _bookRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!await _bookRepository.SaveAsync())
                    throw new Exception($"Adding book {bookId} on save failed");
                var bookDto = _mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor", new {authorId = bookToAdd.AuthorId, bookId = bookToAdd.Id},
                    bookDto);
            }

            var bookToPath = _mapper.Map<Book, BookForUpdateDto>(bookFromRepo);
            jsonPatchDocument.ApplyTo(bookToPath, ModelState);
            
            if (bookToPath.Title==bookToPath.Description)
            {
                ModelState.AddModelError(nameof(BookForUpdateDto), "The provided description shoud be different from the title");
            }

            await TryUpdateModelAsync(bookToPath);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectRestult(ModelState);
            }

            _mapper.Map(bookToPath, bookFromRepo);

            if (!await _bookRepository.SaveAsync())
                throw new Exception($"Patching book {bookId} on save faild");


            return NoContent();
        }
    }
}