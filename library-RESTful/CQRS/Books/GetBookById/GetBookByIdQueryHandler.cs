using library_RESTful.Data;
using library_RESTful.Models;
using MediatR;

namespace library_RESTful.CQRS
{
	public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, CommandResult>
	{
		private readonly LibraryDbContext _context;

		public GetBookByIdQueryHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<CommandResult> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
		{
			// Обработчик получения книги с указанным Id
			var book = await _context.Books.FindAsync(request.Id, cancellationToken);
			return new CommandResult(CommandStatus.Success, value: book);
		}
	}
}
