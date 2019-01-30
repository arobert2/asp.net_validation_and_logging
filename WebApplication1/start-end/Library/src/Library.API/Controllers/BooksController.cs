using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly ILibraryRepository _LibraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _LibraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_LibraryRepository.AuthorExists(authorId))
                return NotFound();
            var repoBooks = _LibraryRepository.GetBooksForAuthor(authorId);
            var books = Mapper.Map<IEnumerable<BookDto>>(repoBooks);
            return Ok(books);
        }

        [HttpGet("{Id}")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid Id)
        {
            if(!_LibraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _LibraryRepository.GetBookForAuthor(authorId, Id);
            if (bookForAuthorFromRepo == null)
                return NotFound();
            var book = Mapper.Map<BookDto>(bookForAuthorFromRepo);
            return Ok(book);
        }

        [HttpPost()]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookForCreationDto book)
        {
            if (book == null)
                return BadRequest();

            if(_LibraryRepository.AuthorExists(authorId))
        }
    }
}