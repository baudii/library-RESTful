using System.Text.Json.Serialization;

namespace library_RESTful.Models
{
	public class Author
	{
		public int Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public DateOnly Birthday { get; set; }

		[JsonIgnore]
		public ICollection<Book> Books { get; set; } = new List<Book>();
	}
}
