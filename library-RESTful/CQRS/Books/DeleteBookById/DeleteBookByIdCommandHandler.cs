using library_RESTful.Data;
using library_RESTful.Models;
using MediatR;

namespace library_RESTful.CQRS
{
	public class DeleteBookByIdCommandHandler : IRequestHandler<DeleteBookByIdCommand, CommandResult>
	{
		private readonly LibraryDbContext _context;
		public DeleteBookByIdCommandHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<CommandResult> Handle(DeleteBookByIdCommand request, CancellationToken cancellationToken)
		{
			// Обработчик удаления книги с указанным Id
			
			var book = await _context.Books.FindAsync(request.Id);
			if (book == null)
				// Если книга с таким Id не существует в БД, возваращаем NotFound
				return new CommandResult(CommandStatus.NotFound);

			// Удаляем книгу из БД и возвращаем Success и экземпляр удаленного объекта
			_context.Books.Remove(book);
			await _context.SaveChangesAsync();
			return new CommandResult(CommandStatus.Success, value: book);
		}
	}
}
