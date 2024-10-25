using library_RESTful.Models;
using MediatR;

namespace library_RESTful.CQRS
{
	public record DeleteBookByIdCommand(int Id) : IRequest<Book?>;
}
