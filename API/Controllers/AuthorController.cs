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
    [Route("api/authors")]
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorController(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {

            //throw new Exception("Just for test");

            var result = await _authorRepository.GetAuthors();
            var authorDto = _mapper.Map<IEnumerable<Author>, IEnumerable<AuthorDto>>(result);
            return Ok(authorDto);
        }

        [HttpGet("{authorId}")]
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
    }
}