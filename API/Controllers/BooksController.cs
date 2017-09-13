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
    [Produces("application/json")]
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetBooksForAuthor(Guid authorId)
        {
            var books = await _bookRepository.GetBooksForAuthor(authorId);
            if (books.Count()==0)
            {
                return NotFound();
            }

            var mappedBooks = _mapper.Map<IEnumerable<Book>, IEnumerable<BookDto>>(books);
            return Ok(mappedBooks);
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetBookForAuthor(Guid authorId, Guid bookId)
        {
            var book = await _bookRepository.GetBookForAuthor(authorId, bookId);
            if (book==null)
            {
                return NotFound();
            }

            var mappedBook = _mapper.Map<Book, BookDto>(book);
            return Ok(mappedBook);
        }
    }
}