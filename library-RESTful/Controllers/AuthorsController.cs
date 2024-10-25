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

		public AuthorsController(ISender sender)
		{
			_sender = sender;
		}

		// GET api/authors
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Author>>> Get()
		{
			var result = await _sender.Send(new GetAuthorsQuery());
			return Ok(result);
		}
	}
}
