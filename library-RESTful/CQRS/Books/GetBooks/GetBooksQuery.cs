using MediatR;
using library_RESTful.Models;

namespace library_RESTful.CQRS
{
	public record GetBooksQuery() : IRequest<CommandResult>;
}
