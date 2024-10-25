namespace library_RESTful.Common
{
	public abstract record CommandResult(object? value);
	public record SuccessCommandResult(object? value = null) : CommandResult(value);
	public record BadRequestCommandResult(object? value = null) : CommandResult(value);
	public record NotFoundCommandResult(object? value = null) : CommandResult(value);

}
