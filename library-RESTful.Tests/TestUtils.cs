using library_RESTful.Data;
using library_RESTful.Models;
using Microsoft.EntityFrameworkCore;

namespace library_RESTful.Tests
{
	public static class TestUtils
	{
		public static async Task<LibraryDbContext> GetTemporaryDbContextAsync()
		{
			var options = new DbContextOptionsBuilder<LibraryDbContext>()
				.UseInMemoryDatabase(databaseName: $"TestDatabase{Guid.NewGuid()}")
				.Options;

			var context = new LibraryDbContext(options);

			await context.Database.EnsureCreatedAsync();


			if (await context.Authors.CountAsync() <= 0)
			{
				for (var i = 1; i < 10; i++)
				{
					var author = GetStandardAuthor();
					context.Authors.Add(author);
				}
				await context.SaveChangesAsync();
			}

			if (await context.Books.CountAsync() <= 0)
			{
				for (var i = 1; i < 10; i++)
				{
					var book = GetStandardBook();
					book.AuthorId = i;
					context.Books.Add(book);
				}
				await context.SaveChangesAsync();
			}

			return context;
		}

		public static Book GetStandardBook()
		{
			return new Book
			{
				Title = "Test",
				PublishedYear = 1999,
				Genre = "Test Genre"
			};
		}

		public static Author GetStandardAuthor()
		{
			return new Author
			{
				FullName = "Test Name ",
				Birthday = new DateOnly(1999, 12, 12)
			};
		}

		public static object? GetAnonymousProperty(this object obj, string propName)
		{
			var type = obj.GetType();
			var prop = type.GetProperty(propName);
			object? val = prop?.GetValue(obj);
			return val;
		}
	}
}
