using library_RESTful.Data;
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
			// Обработчик изменения данных книги с указанным Id

			var dbBook = await _context.Books.FindAsync(request.Id, cancellationToken);
			if (dbBook == null)
				// Если книга с указанным Id не найдена, возвращаем NotFound
				return new CommandResult(CommandStatus.NotFound);

			if (dbBook.AuthorId != request.AuthorId)
			{
				// Если поступил запрос на изменение Id автора, то проверяем, корректен ли новый Id
				var newAuthor = await _context.Authors.FindAsync(request.AuthorId);
				if (newAuthor == null)
					// Если новый Id автора не существует в БД - возвращаем BadRequest
					return new CommandResult(CommandStatus.BadRequest);
			}

			// Обновляем данные книги
			dbBook.Title = request.Title;
			dbBook.PublishedYear = request.PublishedYear;
			dbBook.Genre = request.Genre;
			dbBook.AuthorId = request.AuthorId;

			// Сохраняем данные в БД и возвращаем Success
			_context.Books.Update(dbBook);
			await _context.SaveChangesAsync(cancellationToken);

			return new CommandResult(CommandStatus.Success);
		}
	}
}
