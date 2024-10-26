using FluentAssertions;
using library_RESTful.CQRS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace library_RESTful.Tests.MainTests
{
	public class CommonTests
	{
		[Fact]
		public void ConvertToActionResult_ShouldReturnBadRequestResult_WithStatusAndMessageIsNull()
		{
			// Arrange
			CommandResult commandResult = new CommandResult(CommandStatus.BadRequest, message: null);

			// Act
			var result = commandResult.ConvertToActionResult();

			// Assert
			result.Should().BeOfType(typeof(BadRequestResult));
		}


		[Fact]
		public void ConvertToActionResult_ShouldReturnBadRequestObjectResultWithMessage_WhenMessageIsNotNull()
		{
			// Arrange
			var msg = "Some Message";
			CommandResult commandResult = new CommandResult(CommandStatus.BadRequest, message: msg);

			// Act
			var result = commandResult.ConvertToActionResult();

			// Assert
			result.Should().BeOfType(typeof(BadRequestObjectResult));
			var badReq = result as BadRequestObjectResult;
			badReq!.Value.Should().BeEquivalentTo(msg);
		}


		[Fact]
		public void ConvertToActionResult_ShouldReturnNotFoundResult_WhenMessageIsNull()
		{
			// Arrange
			CommandResult commandResult = new CommandResult(CommandStatus.NotFound, message: null);

			// Act
			var result = commandResult.ConvertToActionResult();

			// Assert
			result.Should().BeOfType(typeof(NotFoundResult));
		}

		[Fact]
		public void ConvertToActionResult_ShouldReturnNotFoundObjectResultWithMessage_WhenMessageIsNotNull()
		{
			// Arrange
			var msg = "Some Message";
			CommandResult commandResult = new CommandResult(CommandStatus.NotFound, message: msg);

			// Act
			var result = commandResult.ConvertToActionResult();

			// Assert
			result.Should().BeOfType(typeof(NotFoundObjectResult));
			var badReq = result as NotFoundObjectResult;
			badReq!.Value.Should().BeEquivalentTo(msg);
		}

		[Fact]
		public void ConvertToActionResult_ShouldReturnStatus500_WhenStatusNotBadRequestAndNotNotFound()
		{
			// Arrange
			CommandResult commandResult1 = new CommandResult(CommandStatus.Success);
			CommandResult commandResult2 = new CommandResult((CommandStatus)99);

			// Act
			var result1 = commandResult1.ConvertToActionResult();
			var result2 = commandResult2.ConvertToActionResult();

			// Assert
			result1.Should().BeOfType(typeof(StatusCodeResult));
			var badReq1 = result1 as StatusCodeResult;
			badReq1!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError); 
			result2.Should().BeOfType(typeof(StatusCodeResult));
			var badReq2 = result2 as StatusCodeResult;
			badReq2!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		}
	}
}
