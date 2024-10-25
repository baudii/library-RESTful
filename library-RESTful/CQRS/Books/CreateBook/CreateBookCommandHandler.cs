using library_RESTful.Data;
using library_RESTful.Models;
using MediatR;

namespace library_RESTful.CQRS
{
	public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Book?>
	{
		private readonly LibraryDbContext _context;
		public CreateBookCommandHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<Book?> Handle(CreateBookCommand request, CancellationToken cancellationToken)
		{
			var author = await _context.Authors.FindAsync(request.AuthorId, cancellationToken);

			if (author == null)
			{
				// BadRequest - Автор не был найден и не предоставлены данные
				if (request.AuthorFullName == null || request.AuthorBirthday == null)
					return null;

				// Создаем автора, если предоставлены данные
				author = new Author
				{
					FullName = request.AuthorFullName!,
					Birthday = (DateOnly)request.AuthorBirthday
				};

				var created = await _context.Authors.AddAsync(author, cancellationToken);
				author = created.Entity;
				await _context.SaveChangesAsync(cancellationToken);
			}

			var book = new Book
			{
				Title = request.Title,
				Genre = request.Genre,
				PublishedYear = request.PublishedYear,
				AuthorId = author.Id
			};

			await _context.Books.AddAsync(book, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			return book;
		}
	}
}
