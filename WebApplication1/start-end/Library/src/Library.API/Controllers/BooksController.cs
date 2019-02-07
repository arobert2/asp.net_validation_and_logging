using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();
            var repoBooks = _libraryRepository.GetBooksForAuthor(authorId);
            var books = Mapper.Map<IEnumerable<BookDto>>(repoBooks);
            return Ok(books);
        }

        [HttpGet("{Id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid Id)
        {
            if(!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, Id);
            if (bookForAuthorFromRepo == null)
                return NotFound();
            var book = Mapper.Map<BookDto>(bookForAuthorFromRepo);
            return Ok(book);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookForCreationDto book)
        {
            if (book == null)
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookEntity = Mapper.Map<Book>(book);

            _libraryRepository.AddBookForAuthor(authorId, bookEntity);
            if (!_libraryRepository.Save())
                throw new Exception($"Creating a book for author {authorId} failed on save.");

            var bookToReturn = Mapper.Map<BookDto>(bookEntity);
            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, Id = bookToReturn.id }, bookToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
                return NotFound();

            _libraryRepository.DeleteBook(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
                throw new Exception("Failed to delete book on Save.");

            return NoContent();

        }
        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
            {
                var bookToAdd = Mapper.Map<Book>(book);
                bookToAdd.Id = id;
                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);
                if (!_libraryRepository.Save())
                    throw new Exception($"Upsering book {id} for author {authorId} failed on save.");

                var bookToReturn = Mapper.Map<BookDto>(bookToAdd);
                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = id }, bookToReturn);
            }

            Mapper.Map(book, bookForAuthorFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);
            if (!_libraryRepository.Save())
                throw new Exception($"Failed to update {book} on save.");

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
                return NotFound();

            var bookToPatch = Mapper.Map<BookForUpdateDto>(bookForAuthorFromRepo);
            patchDoc.ApplyTo(bookToPatch);

            //add validation

            Mapper.Map(bookToPatch, bookForAuthorFromRepo);
            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);
            if (!_libraryRepository.Save())
                throw new Exception($"Patching book {id} for author {authorId} failed on save.");

            return NoContent();

        }
    }
}