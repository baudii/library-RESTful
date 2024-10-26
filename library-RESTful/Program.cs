using library_RESTful.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
		builder.Configuration.GetConnectionString("LibraryDB")
	);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
