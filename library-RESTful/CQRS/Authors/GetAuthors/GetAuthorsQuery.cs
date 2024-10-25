using library_RESTful.Models;
using MediatR;

namespace library_RESTful.CQRS
{
	public record GetAuthorsQuery : IRequest<IEnumerable<Author>>;
}
