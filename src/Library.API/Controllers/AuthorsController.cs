using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        // api/authors
        public IActionResult GetAuthors()
        {
            // throw new Exception("Fake exception for testing"); // this would trigger the global exception handling in Startup.cs

            var authorsFromRepo = _libraryRepository.GetAuthors();
            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
            return Ok(authors);
        }

        [HttpGet("{id}", Name = "GetAuthor")] // api/authors/id
        public IActionResult GetAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if(authorFromRepo == null)
            {
                return NotFound();
            }
            var author = Mapper.Map<AuthorDto>(authorFromRepo);
            return Ok(author);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if(author == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Entities.Author>(author);
            _libraryRepository.AddAuthor(authorEntity);
            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an author failed on save.");
            }

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);
        }
    }
}
