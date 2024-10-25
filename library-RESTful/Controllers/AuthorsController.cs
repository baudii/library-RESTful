using library_RESTful.Models;
using library_RESTful.CQRS;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace library_RESTful.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthorsController : ControllerBase
	{
		private readonly ISender _sender;
		private readonly CancellationTokenSource _cts;

		public AuthorsController(ISender sender)
		{
			_sender = sender;
			_cts = new CancellationTokenSource();
		}

		// GET api/authors
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
		{
			var result = await _sender.Send(new GetAuthorsQuery(), _cts.Token);
			if (result == null)
				return NotFound();
			return Ok(result);
		}
	}
}
