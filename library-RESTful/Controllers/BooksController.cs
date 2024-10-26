using library_RESTful.Models;
using library_RESTful.CQRS;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace library_RESTful.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class BooksController : ControllerBase
	{
		private readonly ISender _sender;
		private CancellationTokenSource _cts;

		public BooksController(ISender sender)
		{
			_sender = sender;
			_cts = new CancellationTokenSource();
		}

		// GET api/books
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
		{
			var result = await _sender.Send(new GetBooksQuery());
			
			if (result == null)
				return NotFound();

			return Ok(result);
		}

		// GET /api/books/{id}
		[HttpGet("{id:int}")]
		public async Task<ActionResult<Book>> GetBook(int id)
		{
			var book = await _sender.Send(new GetBookByIdQuery(id), _cts.Token);

			if (book == null)
				return NotFound();

			return Ok(book);
		}

		// POST api/books
		[HttpPost]
		public async Task<ActionResult<Book>> PostBook(CreateBookCommand command)
		{
			var createResult = await _sender.Send(command, _cts.Token);

			switch (createResult.Status)
			{
				case CommandStatus.Success:
					if (createResult.Value is not Book book)
						break;
					return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
				case CommandStatus.BadRequest:
					return BadRequest(createResult.Message);
			}
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		// PUT api/books/{id}
		[HttpPut]
		public async Task<ActionResult> PutBook(UpdateBookCommand command)
		{
			var putResult = await _sender.Send(command, _cts.Token);

			switch (putResult.Status)
			{
				case CommandStatus.Success:
					return NoContent();
				case CommandStatus.NotFound:
					return NotFound();
				case CommandStatus.BadRequest:
					return BadRequest();
			}
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		// DELETE api/books/{id}
		[HttpDelete("{id:int}")]
		public async Task<ActionResult<Book>> DeleteBook(int id)
		{
			var command = new DeleteBookByIdCommand(id);
			var commandResult = await _sender.Send(command, _cts.Token);

			switch (commandResult.Status)
			{
				case CommandStatus.Success:
					return Ok(commandResult.Value);
				case CommandStatus.NotFound:
					return NotFound();
				case CommandStatus.BadRequest:
					return BadRequest();
			}
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
