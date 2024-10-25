using library_RESTful.Models;
using MediatR;

namespace library_RESTful.CQRS
{
	public record GetBookByIdQuery(int Id) : IRequest<Book?>;
}
