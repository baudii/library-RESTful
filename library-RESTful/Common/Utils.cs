using library_RESTful.Data;
using library_RESTful.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace library_RESTful.Common
{

	public static class Utils
	{
		public static object? GetAnonymousProperty(this object obj, string propName)
		{
			var type = obj.GetType();
			var prop = type.GetProperty(propName);
			object? val = prop?.GetValue(obj);
			return val;
		}

		public static async Task<LibraryDbContext> GetTemporaryDbContextAsync()
		{
			var options = new DbContextOptionsBuilder<LibraryDbContext>()
				.UseInMemoryDatabase(databaseName: $"TestDatabase{Guid.NewGuid()}")
				.Options;

			var context = new LibraryDbContext(options);

			await context.Database.EnsureDeletedAsync();
			await context.Database.EnsureCreatedAsync();


			if (await context.Authors.CountAsync() <= 0)
			{
				for (var i = 1; i < 10; i++)
				{
					context.Authors.Add(
						new Author
						{
							Id = i,
							FullName = "Test Name " + i,
							Birthday = new DateOnly(i, i, 30 - i)
						}
					); ;
				}
				await context.SaveChangesAsync();
			}

			if (await context.Books.CountAsync() <= 0)
			{
				for (var i = 1; i < 10; i++)
				{
					context.Books.Add(
						new Book
						{
							Title = "Test",
							PublishedYear = 1999,
							Genre = "Test Genre",
							AuthorId = i
						}
					);
				}
				await context.SaveChangesAsync();
			}

			return context;
		}
	}
}
