using API.Middleware;
using API.SignalR;
using Application.Activities.Commands;
using Application.Activities.Core;
using Application.Activities.Queries;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt =>
{   //Create An authroization policy that requires users to be authenticated
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

    //Apply the authorization policy globally to all controllers
    opt.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors();

builder.Services.AddSignalR();

//middleware 
builder.Services.AddMediatR(option =>
{
    option.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>(); //scans and registers handlers 
    option.AddOpenBehavior(typeof(ValidationBehaviour<,>)); //Adds validation logic to the request handling pipeline
});

builder.Services.AddScoped<IUserAccessor,UserAccessor>();
builder.Services.AddScoped<IPhotoService,PhotoService>();
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivity>();
//this service is only going to be created when its needed, ie when there is an exception
builder.Services.AddTransient<ExceptionMiddleware>();
//adding the services for for Users
builder.Services.AddIdentityApiEndpoints<User>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthorization(opt =>{
    opt.AddPolicy("IsActivityHost",policy =>{
        policy.Requirements.Add(new IsHostRequirement());
    });
});
builder.Services.AddTransient<IAuthorizationHandler,IsHostRequirementHandler>();
//service for configuring the cloudinary api
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

var app = builder.Build();

//adding the middleware for Error handling
app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
app.UseCors(options => options
    .AllowCredentials()
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<User>(); //api/login
app.MapHub<CommentHub>("/comments");

app.MapFallbackToController("Index","Fallback");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedData(context, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
