using FluentAssertions;
using library_RESTful.CQRS;
using library_RESTful.Models;

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
			result.Status.Should().Be(CommandStatus.BadRequest);
			result.Message.Should().BeOfType(typeof(string));

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
			result.Status.Should().Be(CommandStatus.BadRequest);
			result.Message.Should().BeOfType(typeof(string));

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
			result.Status.Should().Be(CommandStatus.Success);
			result.Value.Should().BeOfType(typeof(Book));
			result.Value.Should().BeEquivalentTo(book, options => options.Excluding(b => b.Id).Excluding(b => b.Author));

			await context.Database.EnsureDeletedAsync();
			context.Dispose();
		}
	}
}
