using API.Middleware;
using Application.Activities.Commands;
using Application.Activities.Core;
using Application.Activities.Queries;
using Application.Core;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors();

//middleware 
builder.Services.AddMediatR(option => {
    option.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>(); //scans and registers handlers 
    option.AddOpenBehavior(typeof(ValidationBehaviour<,>)); //Adds validation logic to the request handling pipeline
});

builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivity>();
//this service is only going to be created when its needed, ie when there is an exception
builder.Services.AddTransient<ExceptionMiddleware>();


var app = builder.Build();

//adding the middleware for Error handling
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedData(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
