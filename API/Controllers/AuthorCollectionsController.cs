using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.DTO;
using Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers;
using Service.Service;

namespace API.Controllers
{
    [Route("api/AuthorCollections")]
    public class AuthorCollectionsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAuthorRepository _authorRepository;

        public AuthorCollectionsController(IMapper mapper, IAuthorRepository authorRepository)
        {
            _mapper = mapper;
            _authorRepository = authorRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthorCollection([FromBody] IList<AuthorForCreation> authorForCreations)
        {
            if (!authorForCreations.Any())
            {
                return BadRequest();
            }

            var author = _mapper.Map<IList<AuthorForCreation>, IList<Author>>(authorForCreations);
            author.ToList().ForEach(async a => await _authorRepository.AddAuthor(a));

            if (!await _authorRepository.SaveAsync())
            {
                return StatusCode(500, "Fail with saving data");
            }

            var authorsDto = _mapper.Map<IList<Author>, IList<AuthorDto>>(author);
            var idsString = string.Join(",", authorsDto.Select(s => s.Id));

            return CreatedAtRoute("GetAuthorCollection", new {authorIds = idsString}, authorsDto);
        }

        [HttpGet("{authorIds}", Name = "GetAuthorCollection")]
        public async Task<IActionResult> GetAuthorCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> authorIds)
        {
            if (!authorIds.Any())
            {
                return BadRequest();
            }

            var authors = await _authorRepository.GetAuthors(authorIds);
            if (authorIds.Count() != authors.Count())
            {
                return NotFound();
            }

            var authorsDto = _mapper.Map<IEnumerable<Author>, IEnumerable<AuthorDto>>(authors);
            return Ok(authorsDto);
        }
    }
}