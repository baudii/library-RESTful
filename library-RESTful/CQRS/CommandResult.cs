namespace library_RESTful.CQRS
{
	public struct CommandResult
	{
		// Структура отображает результат выполнения команды

		public CommandStatus Status;
		public string? Message;
		public object? Value;

		public CommandResult(CommandStatus resultType, object? value = null, string? message = null)
		{
			Message = message;
			Value = value;
			Status = resultType;
		}
	}

	public enum CommandStatus
	{
		Success,
		BadRequest,
		NotFound
	}
}
