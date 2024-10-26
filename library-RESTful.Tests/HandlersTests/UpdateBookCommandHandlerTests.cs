using FluentAssertions;
using library_RESTful.CQRS;
using Microsoft.EntityFrameworkCore;

namespace library_RESTful.Tests.HandlersTests
{
	public class UpdateBookCommandHandlerTests
	{
		[Fact]
		public async Task Handle_ShouldReturnNotFound_WhenBookNotFound()
		{
			// Arrange
			var context = await TestUtils.GetTemporaryDbContextAsync();
			var handler = new UpdateBookCommandHandler(context);

			int id = 9999;
			var command = new UpdateBookCommand(id, "Some", 2000, "Genre", 1);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Status.Should().Be(CommandStatus.NotFound);

			await context.Database.EnsureDeletedAsync();
			context.Dispose();
		}

		[Fact]
		public async Task Handle_ShouldReturnBadRequest_WhenAuthorNotFound()
		{
			// Arrange
			var context = await TestUtils.GetTemporaryDbContextAsync();
			var handler = new UpdateBookCommandHandler(context);

			var existingBook = await context.Books.FirstOrDefaultAsync();
			int nonExistentAuthorId = 9999;
			var command = new UpdateBookCommand(existingBook!.Id, "Updated Title", 2020, "Updated Genre", nonExistentAuthorId);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Status.Should().Be(CommandStatus.BadRequest);

			await context.Database.EnsureDeletedAsync();
			context.Dispose();
		}

		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenBookUpdatedSuccessfully()
		{
			// Arrange
			var context = await TestUtils.GetTemporaryDbContextAsync();
			var handler = new UpdateBookCommandHandler(context);

			var existingBook = await context.Books.FirstOrDefaultAsync();
			var existingAuthor = await context.Authors.FirstOrDefaultAsync();
			var command = new UpdateBookCommand(existingBook!.Id, "Updated Title", 2021, "Updated Genre", existingAuthor!.Id);

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Status.Should().Be(CommandStatus.Success);

			var updatedBook = await context.Books.FindAsync(existingBook.Id);
			updatedBook.Should().NotBeNull();
			updatedBook!.Title.Should().Be("Updated Title");
			updatedBook.PublishedYear.Should().Be(2021);
			updatedBook.Genre.Should().Be("Updated Genre");

			await context.Database.EnsureDeletedAsync();
			context.Dispose();
		}
	}
}
