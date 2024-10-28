using MediatR;

namespace library_RESTful.CQRS
{
	public record GetBooksQuery() : IRequest<CommandResult>;
}
