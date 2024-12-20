﻿using library_RESTful.CQRS;
using library_RESTful.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
			if (result.Status == CommandStatus.Success)
				return Ok(result.Value);

			return result.ConvertToActionResult();
		}
	}
}
