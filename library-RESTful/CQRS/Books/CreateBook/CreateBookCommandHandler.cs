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
				b.AuthorId == author.Id
			);
				b.AuthorId == request.AuthorId
			, token);
		}

			if (bookExists)
			{
				// Если книга с такими данными уже существует, возвращаем BadReqyest
				var result = new CommandResult(CommandStatus.BadRequest, message: $"Identical book already exists");
				return result;
			}
		private async Task<bool> DoesAuthorExist(CreateBookCommand request, CancellationToken token)
		{
			var author = await _context.Authors.FindAsync(request.AuthorId, token);

			return author != null;
		}

			// Создаем экземпляр книги
			var book = new Book
			{
				Title = request.Title,
				Genre = request.Genre,
				PublishedYear = request.PublishedYear,
				AuthorId = author.Id
			};

			// Добавляем в БД и возвращаем Success
			await _context.Books.AddAsync(book, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return new CommandResult(CommandStatus.Success, value: book);
		}
	}
}
