using library_RESTful.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace library_RESTful.CQRS
{
	public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, CommandResult>
	{
		private readonly LibraryDbContext _context;

		public GetBooksQueryHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<CommandResult> Handle(GetBooksQuery request, CancellationToken cancellationToken)
		{
			// Обработчик получения всех книг из БД
			var books = await _context.Books.ToListAsync(cancellationToken);
			return new CommandResult(CommandStatus.Success, value: books);
		}
	}
}
