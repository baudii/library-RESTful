using FluentAssertions;
using library_RESTful.CQRS;
using library_RESTful.Models;
using library_RESTful.Common;

namespace library_RESTful.Tests.HandlersTests
{
	public class CreateBookCommandHandlerTests
	{
		[Fact]
		public async Task Handle_ShouldReturnBadRequest_WhenAuthorNotFound()
		{
			// Arrange
			var command = new CreateBookCommand("Test Title", 2020, "Test Genre", 9999);
			var context = await TestUtils.GetTemporaryDbContextAsync();
			var handler = new CreateBookCommandHandler(context);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().BeOfType(typeof(BadRequestCommandResult));

			await context.Database.EnsureDeletedAsync();
			context.Dispose();
		}

		[Fact]
		public async Task Handle_ShouldReturnBadRequest_WhenBookAlreadyExists()
		{
			// Arrange
			var book = new Book
			{
				Title = "Test",
				PublishedYear = 1999,
				Genre = "Test Genre",
				AuthorId = 1
			};
			var command = new CreateBookCommand(book.Title, book.PublishedYear, book.Genre, book.AuthorId);
			var context = await TestUtils.GetTemporaryDbContextAsync();
			var handler = new CreateBookCommandHandler(context);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().BeOfType<BadRequestCommandResult>();

			await context.Database.EnsureDeletedAsync();
			context.Dispose();
		}


		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenBookCreatedSuccessfully()
		{
			// Arrange
			Book book = new() 
			{ 
				Title = "Unique Titile", 
				PublishedYear = 2020, 
				Genre = "Unique Genre", 
				AuthorId = 1 
			};
			var command = new CreateBookCommand(book.Title, book.PublishedYear, book.Genre, book.AuthorId);
			var context = await TestUtils.GetTemporaryDbContextAsync();
			var handler = new CreateBookCommandHandler(context);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().BeOfType<SuccessCommandResult>();
			var successResult = result as SuccessCommandResult;
			((Book)successResult!.value!).Should().BeEquivalentTo(book, options => options.Excluding(b => b.Id).Excluding(b => b.Author));

			await context.Database.EnsureDeletedAsync();
			context.Dispose();
		}
	}
}
