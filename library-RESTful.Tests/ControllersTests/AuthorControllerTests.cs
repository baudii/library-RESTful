using MediatR;
using FakeItEasy;
using library_RESTful.Controllers;
using library_RESTful.Models;
using library_RESTful.CQRS;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace library_RESTful.Tests.ControllersTests
{
	public class AuthorControllerTests
	{
		private readonly ISender _sender;
		private readonly AuthorsController _authorsController;

		public AuthorControllerTests()
		{
			// Dependancies
			_sender = A.Fake<ISender>();

			// SUT (System Under Test)
			_authorsController = new AuthorsController(_sender);
		}

		[Fact]
		public async void GetAuthors_ReturnsOkObject_WhenAuthorsExists()
		{
			// Arrange
			var authors = new List<Author>
			{
				new Author { Id = 1, FullName = "Test Name 1", Birthday = DateOnly.MinValue },
				new Author { Id = 2, FullName = "Test Name 2", Birthday = DateOnly.MinValue },
				new Author { Id = 3, FullName = "Test Name 3", Birthday = DateOnly.MinValue }
			};
			A.CallTo(() =>
					_sender.Send(A<GetAuthorsQuery>.Ignored, A<CancellationToken>.Ignored)
				).Returns(authors);

			// Act
			var result = await _authorsController.GetAuthors();

			// Assert
			result.Result.Should().BeOfType(typeof(OkObjectResult));
			var okObjResult = result.Result as OkObjectResult;
			okObjResult!.Value.Should().BeEquivalentTo(authors);
		}

		[Fact]
		public async void GetAuthors_ReturnsNotFound_WhenAuthorsDoesNotExists()
		{
			// Arrange
			IEnumerable<Author>? authors = null;
			A.CallTo(() =>
					_sender.Send(A<GetAuthorsQuery>.Ignored, A<CancellationToken>.Ignored)
				)!.Returns(authors);

			// Act
			var result = await _authorsController.GetAuthors();

			// Assert
			result.Result.Should().BeOfType(typeof(NotFoundResult));
		}
	}
}
