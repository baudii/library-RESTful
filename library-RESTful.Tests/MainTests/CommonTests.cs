using FluentAssertions;
using library_RESTful.CQRS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace library_RESTful.Tests.MainTests
{
	public class CommonTests
	{
		[Theory]
		[InlineData(CommandStatus.NotFound, null, "someMessage")]
		[InlineData(CommandStatus.NotFound, 5, "someMessage")]
		[InlineData(CommandStatus.NotFound)]

		public void ConvertToActionResult_ShouldReturnNotFound_WhenStatusIsNotFound(CommandStatus status, object? value = null, string? message = null)
		{
			// Arrange
			CommandResult commandResult = new CommandResult(status, value, message);

			// Act
			var result = commandResult.ConvertToActionResult();

			// Assert
			if (message == null)
			{
				result.Should().BeOfType(typeof(NotFoundResult));
			}
			else
			{
				var notFoundReq = result.Should().BeOfType<NotFoundObjectResult>().Subject;
				notFoundReq!.Value.Should().Be(message);
			}
		}

		[Theory]
		[InlineData(CommandStatus.BadRequest, null, "someMessage")]
		[InlineData(CommandStatus.BadRequest, 5, "someMessage")]
		[InlineData(CommandStatus.BadRequest)]
		public void ConvertToActionResult_ShouldReturnNotBadRequest_WhenStatusIsBadRequest(CommandStatus status, object? value = null, string? message = null)
		{
			// Arrange
			CommandResult commandResult = new CommandResult(status, value, message);

			// Act
			var result = commandResult.ConvertToActionResult();

			// Assert
			if (message == null)
			{
				result.Should().BeOfType(typeof(BadRequestResult));
			}
			else
			{
				var badReq = result.Should().BeOfType<BadRequestObjectResult>().Subject;
				badReq!.Value.Should().BeEquivalentTo(message);
			}
		}

		[Theory]
		[InlineData(CommandStatus.Success)]
		[InlineData((CommandStatus)99)]
		public void ConvertToActionResult_ShouldReturnStatus500_WhenStatusNotBadRequestAndNotNotFound(CommandStatus status, object? value = null, string? message = null)
		{
			// Arrange
			CommandResult commandResult = new CommandResult(status, value, message);

			// Act
			var result = commandResult.ConvertToActionResult();

			// Assert
			var statusCode = result.Should().BeOfType<StatusCodeResult>().Subject;
			statusCode!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError); 
		}
	}
}
