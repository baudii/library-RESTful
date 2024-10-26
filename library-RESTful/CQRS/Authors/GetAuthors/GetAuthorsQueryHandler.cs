using library_RESTful.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace library_RESTful.CQRS.Authors.GetAuthors
{
	public class GetAuthorsQueryHandler : IRequestHandler<GetAuthorsQuery, CommandResult>
	{
		private readonly LibraryDbContext _context;
		public GetAuthorsQueryHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<CommandResult> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
		{
			// Обработчик получения всех авторов из БД
			var authors = await _context.Authors.ToListAsync(cancellationToken);
			return new CommandResult(CommandStatus.Success, value: authors);
		}
	}
}
