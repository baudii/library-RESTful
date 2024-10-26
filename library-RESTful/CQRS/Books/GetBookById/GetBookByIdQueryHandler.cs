using library_RESTful.Data;
using library_RESTful.Models;
using MediatR;

namespace library_RESTful.CQRS
{
	public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Book?>
	{
		private readonly LibraryDbContext _context;

		public GetBookByIdQueryHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<Book?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
		{
			// Обработчик получения книги с указанным Id
			return await _context.Books.FindAsync(request.Id, cancellationToken);
		}
	}
}
