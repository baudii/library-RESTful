namespace library_RESTful.Models
{
	public class Book
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public int PublishedYear { get; set; }
		public string Genre { get; set; } = string.Empty;
		public int AuthorId { get; set; }

		public Author? Author { get; set; }
	}
}
