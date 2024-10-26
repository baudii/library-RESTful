using library_RESTful.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace library_RESTful
{
	public static class Common
	{
		public static ActionResult ConvertToActionResult(this CommandResult commandResult)
		{
			switch (commandResult.Status)
			{
				case CommandStatus.BadRequest:
					if (commandResult.Message != null)
						return new BadRequestObjectResult(commandResult.Message);
					return new BadRequestResult();
				case CommandStatus.NotFound:
					if (commandResult.Message != null)
						return new NotFoundObjectResult(commandResult.Message);
					return new NotFoundResult();
			}

			return new StatusCodeResult(StatusCodes.Status500InternalServerError);
		}
	}
}
