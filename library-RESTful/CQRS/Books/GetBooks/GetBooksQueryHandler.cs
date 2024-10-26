using library_RESTful.Data;
using library_RESTful.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace library_RESTful.CQRS
{
	public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, IEnumerable<Book>>
	{
		private readonly LibraryDbContext _context;

		public GetBooksQueryHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Book>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
		{
			// Обработчик получения всех книг из БД
			return await _context.Books.ToListAsync(cancellationToken);
		}
	}
}
