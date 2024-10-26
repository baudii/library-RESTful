using MediatR;

namespace library_RESTful.CQRS
{
	public record GetAuthorsQuery : IRequest<CommandResult>;
}
