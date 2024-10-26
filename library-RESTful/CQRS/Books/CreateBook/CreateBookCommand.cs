using MediatR;
using System.ComponentModel.DataAnnotations;

namespace library_RESTful.CQRS
{
    public record CreateBookCommand(
		[Required(ErrorMessage = "Title is required")]
		[MaxLength(50, ErrorMessage = "Title can't be longer than 50 characters")]
		string Title,
		[Range(600, 2024, ErrorMessage = "PublishedYear must be between 600 and 2024")]
		int PublishedYear,
		[Required(ErrorMessage = "Genre is required")]
		string Genre,
		[Range(1, int.MaxValue, ErrorMessage = "AuthorId must be positive integer")]
		int AuthorId
	) : IRequest<CommandResult>;
}
