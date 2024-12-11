using library_RESTful.Data;
using library_RESTful.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace library_RESTful.CQRS
{
	public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, CommandResult>
	{
		private readonly LibraryDbContext _context;
		public CreateBookCommandHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<CommandResult> Handle(CreateBookCommand request, CancellationToken token)
		{
			if (!await DoesAuthorExist(request, token))
				return new CommandResult(CommandStatus.BadRequest, message: $"Author with id={request.AuthorId} doesn't exist");

			if (await DoesBookExist(request, token))
				return new CommandResult(CommandStatus.BadRequest, message: $"Identical book already exists");

			var book = await CreateBook(request, token);
			return new CommandResult(CommandStatus.Success, value: book);
		}

		private async Task<bool> DoesBookExist(CreateBookCommand request, CancellationToken token)
		{
			return await _context.Books.AnyAsync(b =>
				b.Title == request.Title &&
				b.Genre == request.Genre &&
				b.PublishedYear == request.PublishedYear &&
				b.AuthorId == request.AuthorId
			, token);
		}

		private async Task<bool> DoesAuthorExist(CreateBookCommand request, CancellationToken token)
		{
			var author = await _context.Authors.FindAsync(request.AuthorId, token);

			return author != null;
		}

		private async Task<Book> CreateBook(CreateBookCommand request, CancellationToken token)
		{
			var book = new Book
			{
				Title = request.Title,
				Genre = request.Genre,
				PublishedYear = request.PublishedYear,
				AuthorId = request.AuthorId
			};

			var result = await _context.Books.AddAsync(book, token);
			await _context.SaveChangesAsync(token);
			return result.Entity;
		}
	}
}
