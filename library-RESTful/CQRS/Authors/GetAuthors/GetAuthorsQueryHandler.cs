using library_RESTful.Data;
using library_RESTful.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace library_RESTful.CQRS.Authors.GetAuthors
{
	public class GetAuthorsQueryHandler : IRequestHandler<GetAuthorsQuery, IEnumerable<Author>>
	{
		private readonly LibraryDbContext _context;
		public GetAuthorsQueryHandler(LibraryDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Author>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
		{
			return await _context.Authors.ToListAsync(cancellationToken);
		}
	}
}
