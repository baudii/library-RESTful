using MediatR;
using System.ComponentModel.DataAnnotations;

namespace library_RESTful.CQRS
{
	public record DeleteBookByIdCommand(
		[Range(1, int.MaxValue, ErrorMessage = "Id must be positive integer")]
		int Id
	) : IRequest<CommandResult>;
}
