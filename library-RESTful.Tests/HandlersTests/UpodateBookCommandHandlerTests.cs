using FluentAssertions;
using library_RESTful.CQRS;
using library_RESTful.Models;
using library_RESTful.Common;

namespace library_RESTful.Tests.HandlersTests
{
	public class UpodateBookCommandHandlerTests
	{
		[Fact]
		public async Task Handle_ShouldReturnNull_WhenBookNotFound()
		{
			// Arrange
			var context = await Utils.GetTemporaryDbContextAsync();
			var handler = new UpdateBookCommandHandler(context);

			int id = 9999;
			var command = new UpdateBookCommand(id, "Some", "Genre", 1);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().BeOfType(typeof(NotFoundCommandResult));
			context.Dispose();
		}


		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenBookDeletedSuccessfully()
		{
			// Arrange
			var context = await Utils.GetTemporaryDbContextAsync();
			var handler = new DeleteBookByIdCommandHandler(context);
			
			int id = 1;
			var command = new DeleteBookByIdCommand(id);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().BeOfType(typeof(Book));
			context.Dispose();
		}
	}
}
