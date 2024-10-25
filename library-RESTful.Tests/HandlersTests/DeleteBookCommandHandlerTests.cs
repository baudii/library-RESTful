using FluentAssertions;
using library_RESTful.CQRS;
using library_RESTful.Models;

namespace library_RESTful.Tests.HandlersTests
{
	public class DeleteBookCommandHandlerTests
	{

		[Fact]
		public async Task Handle_ShouldReturnNull_WhenBookDoesNotExist()
		{
			// Arrange
			var context = await TestUtils.GetTemporaryDbContextAsync();
			var handler = new DeleteBookByIdCommandHandler(context);
			
			int id = 9999;
			var command = new DeleteBookByIdCommand(id);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().BeNull();

			await context.Database.EnsureDeletedAsync();
			context.Dispose();
		}


		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenBookDeletedSuccessfully()
		{
			// Arrange
			var context = await TestUtils.GetTemporaryDbContextAsync();
			var handler = new DeleteBookByIdCommandHandler(context);
			
			int id = 1;
			var command = new DeleteBookByIdCommand(id);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().BeOfType(typeof(Book));

			await context.Database.EnsureDeletedAsync();
			context.Dispose();
		}
	}
}
