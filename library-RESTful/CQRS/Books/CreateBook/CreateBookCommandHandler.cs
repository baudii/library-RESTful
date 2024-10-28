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

		public async Task<CommandResult> Handle(CreateBookCommand request, CancellationToken cancellationToken)
		{
			// Обработчик создания книг
			var author = await _context.Authors.FindAsync(request.AuthorId, cancellationToken);

			if (author == null)
			{
				// Предполагаем, что автор существует и корректно указан, иначе возвращаем BadRequest
				var result = new CommandResult(CommandStatus.BadRequest, message: $"Author with id={request.AuthorId} doesn't exist");
				return result;
			}


			// Проверяем существует ли книга с такими данными
			var bookExists = await _context.Books.AnyAsync(b =>
				b.Title == request.Title &&
				b.Genre == request.Genre &&
				b.PublishedYear == request.PublishedYear &&
				b.AuthorId == author.Id
			);

			if (bookExists)
			{
				// Если книга с такими данными уже существует, возвращаем BadReqyest
				var result = new CommandResult(CommandStatus.BadRequest, message: $"Identical book already exists");
				return result;
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
