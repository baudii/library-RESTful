using library_RESTful.Data;
using library_RESTful.Common;
using MediatR;

namespace library_RESTful.CQRS
{
	public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, CommandResult>
	{

		private readonly LibraryDbContext _context;
		public UpdateBookCommandHandler(LibraryDbContext context)
		{
			_context = context;
		}
		public async Task<CommandResult> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
		{
			var dbBook = await _context.Books.FindAsync(request.Id, cancellationToken);

			if (dbBook == null)
				return new NotFoundCommandResult();

			if (dbBook.AuthorId != request.AuthorId)
			{
				var newAuthor = await _context.Authors.FindAsync(request.AuthorId);
				if (newAuthor == null)
					return new BadRequestCommandResult();
			}

			dbBook.Title = request.Title;
			dbBook.PublishedYear = request.PublishedYear;
			dbBook.Genre = request.Genre;
			dbBook.AuthorId = request.AuthorId;

			_context.Books.Update(dbBook);
			await _context.SaveChangesAsync(cancellationToken);

			return new SuccessCommandResult();
		}
	}
}
