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
		public async void GetBooks_ReturnsOk_WhenBooksExists()
		{
			// Arrange
			var fakeBooks = new List<Book>
			{
				new Book { Id = 1, Title = "Test Book 1" },
				new Book { Id = 2, Title = "Test Book 2" }
			};
			var bookQuery = new GetBooksQuery();
			var queryResult = new CommandResult(CommandStatus.Success, fakeBooks);
			A.CallTo(() =>
					_sender.Send(A<GetBooksQuery>.Ignored, A<CancellationToken>.Ignored)
				)!.Returns(queryResult);

			// Act
			var result = await _booksController.GetBooks();

			// Assert
			result.Should().BeOfType(typeof(ActionResult<IEnumerable<Book>>));
			result.Result.Should().BeOfType(typeof(OkObjectResult));
			var books = result.Result as OkObjectResult;
			books!.Value.Should().BeEquivalentTo(fakeBooks);
		}

		[Fact]
		public async void GetBooks_ReturnsNotFound_WhenBooksDoesNotExist()
		{
			// Arrange
			IEnumerable<Book>? fakeBooks = null;
			var queryResult = new CommandResult(CommandStatus.NotFound, value: fakeBooks);
			A.CallTo(() => 
					_sender.Send(A<GetBooksQuery>.Ignored, A<CancellationToken>.Ignored)
				)!.Returns(queryResult);

			// Act
			var result = await _booksController.GetBooks();

			// Assert
			result.Result.Should().BeOfType(typeof(NotFoundResult));
		}

		[Fact]
		public async void GetBook_ReturnsOkWithBook_WhenBookExists()
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

			A.CallTo(() => 
						_sender.Send(A<GetBookByIdQuery>.That.Matches(query => query.Id == bookId), A<CancellationToken>.Ignored)
					).Returns(queryResult);

			// Act
			var result = await _booksController.GetBook(bookId);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			okResult!.Value.Should().BeEquivalentTo(fakeBook);
		}

		[Fact]
		public async void GetBook_ReturnsNotFound_WhenBookDoesNotExists()
		{
			// Arrange
			var bookId = 1;
			Book? fakeBook = null;
			var bookQueryById = new GetBookByIdQuery(bookId);
			var queryResult = new CommandResult(CommandStatus.NotFound, value: fakeBook);

			A.CallTo(() => _sender.Send(A<GetBookByIdQuery>.That.Matches(query => query.Id == bookId), A<CancellationToken>.Ignored))
				.Returns(queryResult);

			// Act
			var result = await _booksController.GetBook(bookId);

			// Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}

		#endregion

		#region Post.Tests

		[Fact]
		public async void PostBook_ReturnsCreatedAtActionResult_WhenCreatedBookExists()
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
			result.Result.Should().BeOfType(typeof(CreatedAtActionResult));
			var created = result.Result as CreatedAtActionResult;
			created!.Value.Should().BeEquivalentTo(fakeBook);
		}

		[Fact]
		public async void PostBook_ReturnsBadRequestObjectResult_WhenCreatedBookAlreadyExists()
		{
			// Arrange
			var errorMessage = "SomeMessage";
			var fakeResult = new CommandResult(CommandStatus.BadRequest, message: errorMessage);
			var command = new CreateBookCommand("Test", 2000, "TestGenre", 2);
			A.CallTo(() => _sender.Send(command, A<CancellationToken>.Ignored))
				.Returns(fakeResult);

			// Act
			var result = await _booksController.PostBook(command);

			// Assert
			result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
			var badRequest = result.Result as BadRequestObjectResult;
			string msg = badRequest!.Value!.ToString()!;
			msg.Should().BeEquivalentTo(errorMessage);
		}

		[Fact]
		public async void PostBook_Returns500_WhenGivenResultOrValueNull()
		{
			// Arrange
			CommandResult fakeResult = new CommandResult(CommandStatus.Success, value: null);

			var command1 = new CreateBookCommand("Test1", 2000, "TestGenre1", 2);
			var command2 = new CreateBookCommand("Test2", 2001, "TestGenre2", 3);

			A.CallTo(() => _sender.Send(command2, A<CancellationToken>.Ignored))!
				.Returns(fakeResult);

			// Act
			var task1 = _booksController.PostBook(command1);
			var task2 = _booksController.PostBook(command2);

			await Task.WhenAll(task1, task2);

			var result1 = task1.Result;
			var result2 = task2.Result;

			// Assert
			result1.Result.Should().BeOfType(typeof(StatusCodeResult));
			result2.Result.Should().BeOfType(typeof(StatusCodeResult));
			var statusCode1 = result1.Result as StatusCodeResult;
			var statusCode2 = result2.Result as StatusCodeResult;
			statusCode1!.StatusCode.Should().Be(500);
			statusCode2!.StatusCode.Should().Be(500);
		}

		#endregion

		#region Put.Tests

		[Fact]
		public async void PutBook_ReturnsNoContentResult_WhenResultStatusSuccess()
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

		[Fact]
		public async void PutBook_ReturnsNotFoundResult_WhenResultStatusNotFound()
		{
			// Arrange
			var resultStatus = new CommandResult(CommandStatus.NotFound);
			var command = new UpdateBookCommand(1, "Test", 2000, "TestGenre", 2);
			A.CallTo(() => _sender.Send(command, A<CancellationToken>.Ignored))
				.Returns(resultStatus);

			// Act
			var result = await _booksController.PutBook(command);

			// Assert
			result.Should().BeOfType(typeof(NotFoundResult));
		}

		[Fact]
		public async void PutBook_ReturnsBadRequestResult_WhenResultStatusBadRequest()
		{
			// Arrange
			CommandResult resultStatus = new CommandResult(CommandStatus.BadRequest);
			var command = new UpdateBookCommand(1, "Test", 2000, "TestGenre", 2);
			A.CallTo(() => _sender.Send(command, A<CancellationToken>.Ignored))
				.Returns(resultStatus);

			// Act
			var result = await _booksController.PutBook(command);

			// Assert
			result.Should().BeOfType(typeof(BadRequestResult));
		}

		[Fact]
		public async void PutBook_ReturnsStatus500_WhenBadStatus()
		{
			// Arrange
			CommandResult resultStatus = new CommandResult((CommandStatus)999);
			var command = new UpdateBookCommand(1, "Test", 2000, "TestGenre", 2);
			A.CallTo(() => _sender.Send(command, A<CancellationToken>.Ignored))!
				.Returns(resultStatus);

			// Act
			var result = await _booksController.PutBook(command);

			// Assert
			result.Should().BeOfType(typeof(StatusCodeResult));
			var statusCode = result as StatusCodeResult;
			statusCode!.StatusCode.Should().Be(500);
		}

		#endregion

		#region Delete.Tests

		[Fact]
		public async void DeleteBook_ReturnsOkWithBook_WhenBookWasFound()
		{
			// Arrange
			int fakeId = 4;
			Book? fakeBook = new Book
			{
				Id = 7,
				Title = "Test_Title"
			};

			var fakeResult = new CommandResult(CommandStatus.Success, value: fakeBook);
			A.CallTo(() => 
					_sender.Send(A<DeleteBookByIdCommand>.That.Matches(
							command => command.Id == fakeId
						), A<CancellationToken>.Ignored)
				).Returns(fakeResult);

			// Act
			var result = await _booksController.DeleteBook(fakeId);

			// Assert
			result.Result.Should().BeOfType(typeof(OkObjectResult));
			var okObject = result.Result as OkObjectResult;
			okObject!.Value.Should().Be(fakeBook);
		}

		[Fact]
		public async void DeleteBook_ReturnsNotFoundResult_WhenBookWasNotFound()
		{
			// Arrange
			int fakeId = 4;
			var fakeResult = new CommandResult(CommandStatus.NotFound, value: null);
			A.CallTo(() =>
					_sender.Send(A<DeleteBookByIdCommand>.That.Matches(
							command => command.Id == fakeId
						), A<CancellationToken>.Ignored)
				).Returns(fakeResult);

			// Act
			var result = await _booksController.DeleteBook(fakeId);

			// Assert
			result.Result.Should().BeOfType(typeof(NotFoundResult));
		}

		#endregion
	}
}
