using library_RESTful.Models;
using library_RESTful.CQRS;
using library_RESTful.Common;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace library_RESTful.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BooksController : ControllerBase
	{
		private readonly ISender _sender;

		public BooksController(ISender sender)
		{
			_sender = sender;
		}

		// GET api/books
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Book>>> GetBook()
		{
			var result = await _sender.Send(new GetBooksQuery());
			return Ok(result);
		}

		// GET /api/books/{id}
		[HttpGet("{id:int}")]
		public async Task<ActionResult<Book>> GetBooks(int id)
		{
			var book = await _sender.Send(new GetBookByIdQuery(id));

			if (book == null)
				return NotFound();

			return book;
		}

		// POST api/books
		[HttpPost]
		public async Task<ActionResult<Book>> PostBook(CreateBookCommand command)
		{
			var createdBook = await _sender.Send(command);
			if (createdBook == null)
				return BadRequest(new { Message = $"Author with id={command.AuthorId} doesn't exist" });

			return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
		}

		// PUT api/books/{id}
		[HttpPut]
		public async Task<ActionResult> PutBook(UpdateBookCommand command)
		{
			var resultStatus = await _sender.Send(command);
			switch (resultStatus)
			{
				case Utils.ResultStatus.Success:
					return NoContent();
				case Utils.ResultStatus.NotFound:
					return NotFound();
				case Utils.ResultStatus.BadRequest:
					return BadRequest();
			}
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		// DELETE api/books/{id}
		[HttpDelete("{id:int}")]
		public async Task<ActionResult<Book>> DeleteBook(int id)
		{
			var command = new DeleteBookByIdCommand(id);
			var book = await _sender.Send(command);
			if (book == null)
				return NotFound();

			return Ok(book);
		}
	}
}
