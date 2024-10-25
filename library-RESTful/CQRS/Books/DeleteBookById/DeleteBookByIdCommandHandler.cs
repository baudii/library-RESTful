using library_RESTful.Data;
using library_RESTful.Models;
using MediatR;

namespace library_RESTful.CQRS
{
	public class DeleteBookByIdCommandHandler : IRequestHandler<DeleteBookByIdCommand, Book?>
	{
		private readonly LibraryDbContext _context;
		public DeleteBookByIdCommandHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<Book?> Handle(DeleteBookByIdCommand request, CancellationToken cancellationToken)
		{
			var book = await _context.Books.FindAsync(request.Id);

			if (book == null)
				return null;

			_context.Books.Remove(book);
			await _context.SaveChangesAsync();
			return book;
		}
	}
}
