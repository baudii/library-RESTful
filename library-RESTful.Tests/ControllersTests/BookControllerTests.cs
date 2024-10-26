using MediatR;
using FakeItEasy;
using library_RESTful.Controllers;
using library_RESTful.Models;
using library_RESTful.CQRS;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace library_RESTful.Tests.ControllersTests
{
	public class BookControllerTests
	{
		private readonly ISender _sender;
		private readonly BooksController _booksController;

		public BookControllerTests()
		{
			// Dependancies
			_sender = A.Fake<ISender>();

			// SUT (System Under Test)
			_booksController = new BooksController(_sender);
		}

		#region Get.Tests

		[Fact]
		public async Task GetBooks_ReturnsOk_WhenBooksExists()
		{
			// Arrange
			var fakeBooks = new List<Book>
			{
				new Book { Id = 1, Title = "Test Book 1" },
				new Book { Id = 2, Title = "Test Book 2" }
			};
			var bookQuery = new GetBooksQuery();
			var queryResult = new CommandResult(CommandStatus.Success, fakeBooks);
			A.CallTo(() =>_sender.Send(A<GetBooksQuery>.Ignored, A<CancellationToken>.Ignored))!
				.Returns(queryResult);

			// Act
			var result = await _booksController.GetBooks();

			// Assert
			var books = result.Result.Should().BeOfType<OkObjectResult>().Subject;
			books!.Value.Should().BeEquivalentTo(fakeBooks);
		}

		[Theory]
		[InlineData(CommandStatus.NotFound)]
		[InlineData(CommandStatus.BadRequest)]
		[InlineData((CommandStatus)999)]
		public async Task GetBooks_ReturnsBadActionResult_WhenStatusNotSuccess(CommandStatus status)
		{
			// Arrange
			var commandResult = new CommandResult(status, null, null);
			var expected = commandResult.ConvertToActionResult();
			A.CallTo(() => _sender.Send(A<GetBooksQuery>.Ignored, A<CancellationToken>.Ignored))
				.Returns(commandResult);

			// Act
			var result = await _booksController.GetBooks();

			// Assert
			result.Result.Should().BeOfType(expected.GetType());
		}

		[Fact]
		public async Task GetBook_ReturnsOkWithBook_WhenBookExists()
		{
			// Arrange
			var bookId = 1;
			var fakeBook = new Book
			{
				Id = bookId,
				Title = "Test"
			};
			var bookQueryById = new GetBookByIdQuery(bookId);
			var queryResult = new CommandResult(CommandStatus.Success, value: fakeBook);

			A.CallTo(() => _sender.Send(A<GetBookByIdQuery>.That.Matches(query => query.Id == bookId), A<CancellationToken>.Ignored))
				.Returns(queryResult);

			// Act
			var result = await _booksController.GetBook(bookId);

			// Assert
			var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
			okResult!.Value.Should().BeEquivalentTo(fakeBook);
		}

		[Theory]
		[InlineData(CommandStatus.NotFound)]
		[InlineData(CommandStatus.NotFound, "Some Msg")]
		[InlineData(CommandStatus.BadRequest)]
		[InlineData(CommandStatus.BadRequest, "Some Msg")]
		[InlineData((CommandStatus)999)]
		public async Task GetBook_ReturnsBadActionResult_WhenStatusNotSuccess(CommandStatus status, string? message = null)
		{
			// Arrange
			var bookId = 1;
			var queryResult = new CommandResult(status, message: message);
			var expected = queryResult.ConvertToActionResult();

			A.CallTo(() => _sender.Send(A<GetBookByIdQuery>.That.Matches(query => query.Id == bookId), A<CancellationToken>.Ignored))
				.Returns(queryResult);

			// Act
			var result = await _booksController.GetBook(bookId);

			// Assert
			result.Result.Should().BeOfType(expected.GetType());
			if (result.Result is ObjectResult objResult)
				objResult.Value.Should().BeEquivalentTo(message);
		}

		#endregion

		#region Post.Tests

		[Fact]
		public async Task PostBook_ReturnsCreatedAtWithBook_WhenSuccessResultWithBook()
		{
			// Arrange
			var fakeBook = new Book
			{
				Id = 1,
				Title = "Test"
			};
			var fakeResult = new CommandResult(CommandStatus.Success, value: fakeBook);
			var command = new CreateBookCommand("Test", 2000, "TestGenre", 2);
			A.CallTo(() => _sender.Send(command, A<CancellationToken>.Ignored))
				.Returns(fakeResult);

			// Act
			var result = await _booksController.PostBook(command);

			// Assert
			var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
			createdResult!.Value.Should().BeEquivalentTo(fakeBook);
		}

		[Theory]
		[InlineData(CommandStatus.NotFound)]
		[InlineData(CommandStatus.BadRequest)]
		[InlineData(CommandStatus.Success)]
		[InlineData((CommandStatus)999)]
		public async Task PostBook_ReturnsBadAction_WhenStatusIsNotSuccessOrValueIsNotBook(CommandStatus status)
		{
			// Arrange
			var errorMessage = "SomeMessage";
			var fakeResult = new CommandResult(status, message: errorMessage);
			var command = new CreateBookCommand("Test", 2000, "TestGenre", 2);
			var expected = fakeResult.ConvertToActionResult();

			A.CallTo(() => _sender.Send(command, A<CancellationToken>.Ignored))
				.Returns(fakeResult);

			// Act
			var result = await _booksController.PostBook(command);

			// Assert
			result.Result.Should().BeOfType(expected.GetType());
			if (result.Result is ObjectResult objResult)
				objResult.Value.Should().BeEquivalentTo(errorMessage);
		}

		#endregion

		#region Put.Tests

		[Fact]
		public async Task PutBook_ReturnsNoContentResult_WhenResultStatusSuccess()
		{
			// Arrange
			var resultStatus = new CommandResult(CommandStatus.Success);
			var command = new UpdateBookCommand(1, "Test", 2000, "TestGenre", 2);
			A.CallTo(() => _sender.Send(command, A<CancellationToken>.Ignored))
				.Returns(resultStatus);

			// Act
			var result = await _booksController.PutBook(command);

			// Assert
			result.Should().BeOfType(typeof(NoContentResult));
		}

		[Theory]
		[InlineData(CommandStatus.NotFound)]
		[InlineData(CommandStatus.BadRequest)]
		[InlineData((CommandStatus)999)]
		public async Task PutBook_ReturnsBadResult_WhenBadCommandStatus(CommandStatus status)
		{
			// Arrange
			var message = "Test";
			var resultStatus = new CommandResult(status, message: message);
			var command = new UpdateBookCommand(1, "Test", 2000, "TestGenre", 2);
			var expected = resultStatus.ConvertToActionResult();
			A.CallTo(() => _sender.Send(command, A<CancellationToken>.Ignored))
				.Returns(resultStatus);

			// Act
			var result = await _booksController.PutBook(command);

			// Assert
			result.Should().BeOfType(expected.GetType());
			if (result is ObjectResult objResult)
				objResult.Value.Should().BeEquivalentTo(message);
		}

		#endregion

		#region Delete.Tests
		 
		[Fact]
		public async Task DeleteBook_ReturnsOkWithBook_WhenCommandStatusSuccess()
		{
			// Arrange
			int fakeId = 4;
			Book? fakeBook = new Book
			{
				Id = 7,
				Title = "Test_Title"
			};

			var fakeResult = new CommandResult(CommandStatus.Success, value: fakeBook);
			A.CallTo(() => _sender.Send(A<DeleteBookByIdCommand>
				.That.Matches(command => command.Id == fakeId), A<CancellationToken>.Ignored))
				.Returns(fakeResult);

			// Act
			var result = await _booksController.DeleteBook(fakeId);

			// Assert
			result.Result.Should().BeOfType(typeof(OkObjectResult));
			var okObject = result.Result as OkObjectResult;
			okObject!.Value.Should().Be(fakeBook);
		}


		[Theory]
		[InlineData(CommandStatus.NotFound)]
		[InlineData(CommandStatus.BadRequest)]
		[InlineData((CommandStatus)999)]
		public async Task DeleteBook_ReturnsBadResult_WhenStatusNotSuccess(CommandStatus status)
		{
			// Arrange
			int fakeId = 4;
			var message = "Test";
			var fakeResult = new CommandResult(status, message: message);
			var expected = fakeResult.ConvertToActionResult();
			A.CallTo(() => _sender.Send(A<DeleteBookByIdCommand>
				.That.Matches(command => command.Id == fakeId), A<CancellationToken>.Ignored))
				.Returns(fakeResult);

			// Act
			var result = await _booksController.DeleteBook(fakeId);

			// Assert
			result.Result.Should().BeOfType(expected.GetType());
			if (result.Result is ObjectResult objResult) 
				objResult.Value.Should().BeEquivalentTo(message);
		}

		#endregion
	}
}
