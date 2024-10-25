using library_RESTful.Models;
using Microsoft.EntityFrameworkCore;

namespace library_RESTful.Data
{
	public class LibraryDbContext : DbContext
	{
		public DbSet<Book> Books { get; set; }
		public DbSet<Author> Authors { get; set; }

		public LibraryDbContext(DbContextOptions options) : base(options) { }
	}
}
