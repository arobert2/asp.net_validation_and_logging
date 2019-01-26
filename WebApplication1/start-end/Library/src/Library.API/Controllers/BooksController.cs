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
    public class BooksController : Controller
    {
        private readonly ILibraryRepository _LibraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _LibraryRepository = libraryRepository;
        }

        [HttpGet("api/authors/{authorId}/books")]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_LibraryRepository.AuthorExists(authorId))
                return NotFound();
            var repoBooks = _LibraryRepository.GetBooksForAuthor(authorId);
            var books = Mapper.Map<IEnumerable<BookDto>>(repoBooks);
            return Ok(books);
        }

        [HttpGet("api/authors/{authorId}/books/{bookId}")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid bookId)
        {
            if(!_LibraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _LibraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookForAuthorFromRepo == null)
                return NotFound();
            var book = Mapper.Map<BookDto>(bookForAuthorFromRepo);
            return Ok(book);
        }
    }
}