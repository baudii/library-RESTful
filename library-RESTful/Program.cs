using library_RESTful.Data;
using Microsoft.EntityFrameworkCore;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

string? docker_env = Environment.GetEnvironmentVariable("IS_DOCKER");
string libConnectionStringName = "LibraryDB";

if (!string.IsNullOrEmpty(docker_env))
{
	// Мы внутри Docker
	// Отключаем использование HTTPS
	builder.WebHost.ConfigureKestrel(serverOptions =>
	{
		serverOptions.ListenAnyIP(5000);  // HTTP порт
	});
	libConnectionStringName += "_Docker";
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddMediatR(config =>
{
	config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
	options.UseNpgsql(
		builder.Configuration.GetConnectionString(libConnectionStringName)
	);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

/*
 * 
 * {
  "title": "The Great Gatsby",
  "publishedYear": 1925,
  "genre": "Fiction",
  "authorId": 1,
  "authorFullName": "F. Scott Fitzgerald",
  "authorBirthday": "1896-09-24"
}

 * 
 * */