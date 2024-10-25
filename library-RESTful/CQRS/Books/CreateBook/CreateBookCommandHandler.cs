using library_RESTful.Data;
using library_RESTful.Models;
using library_RESTful.Common;
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
			var author = await _context.Authors.FindAsync(request.AuthorId, cancellationToken);

			if (author == null)
				return new BadRequestCommandResult($"Author with id={request.AuthorId} doesn't exist");

			var bookExists = await _context.Books.AnyAsync(b =>
				b.Title == request.Title &&
				b.Genre == request.Genre &&
				b.PublishedYear == request.PublishedYear &&
				b.AuthorId == author.Id
			);

			if (bookExists)
				return new BadRequestCommandResult($"Identical book already exists");

			var book = new Book
			{
				Title = request.Title,
				Genre = request.Genre,
				PublishedYear = request.PublishedYear,
				AuthorId = author.Id
			};

			
			await _context.Books.AddAsync(book, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			return new SuccessCommandResult(book);
		}
	}
}
