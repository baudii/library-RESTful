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
			var getBooksResult = await _sender.Send(new GetBooksQuery());
			
			if (getBooksResult.Status == CommandStatus.Success)
				return Ok(getBooksResult.Value);

			return getBooksResult.ConvertToActionResult();
		}

		// GET /api/books/{id}
		[HttpGet("{id:int}")]
		public async Task<ActionResult<Book>> GetBook(int id)
		{
			var getBookByIdResult = await _sender.Send(new GetBookByIdQuery(id), _cts.Token);

			if (getBookByIdResult.Status == CommandStatus.Success)
				return Ok(getBookByIdResult.Value);

			return getBookByIdResult.ConvertToActionResult();
		}

		// POST api/books
		[HttpPost]
		public async Task<ActionResult<Book>> PostBook(CreateBookCommand command)
		{
			var createResult = await _sender.Send(command, _cts.Token);

			if (createResult.Status == CommandStatus.Success && createResult.Value is Book book)
				return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);

			return createResult.ConvertToActionResult();
		}

		// PUT api/books/{id}
		[HttpPut]
		public async Task<ActionResult> PutBook(UpdateBookCommand command)
		{
			var putResult = await _sender.Send(command, _cts.Token);

			if (putResult.Status == CommandStatus.Success)
				return NoContent();

			return putResult.ConvertToActionResult();
		}

		// DELETE api/books/{id}
		[HttpDelete("{id:int}")]
		public async Task<ActionResult<Book>> DeleteBook(int id)
		{
			var command = new DeleteBookByIdCommand(id);
			var deleteResult = await _sender.Send(command, _cts.Token);

			if (deleteResult.Status == CommandStatus.Success)
				return Ok(deleteResult.Value);

			return deleteResult.ConvertToActionResult();
		}
	}
}
