using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Shared.Data;
using OnlineBookStore.Api.Modules.Books.Interfaces;
using OnlineBookStore.Api.Modules.Books.Services;
using OnlineBookStore.Api.Shared.Middleware;
using Serilog;
using OnlineBookStore.Api.Modules.Auth.Interfaces;
using OnlineBookStore.Api.Modules.Auth.Services;

namespace OnlineBookStore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext() // we added the Enrich.FromLogContext() method to our Serilog configuration. This allows us to include contextual information (like the request ID) in our log entries by pushing properties into the log context during request processing.
                .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] RequestId = {RequestId} - {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] RequestId = {RequestId} - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();


            builder.Host.UseSerilog();

            // Add services to the container.

            builder.Services.AddControllers();
            
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddScoped<IBookService, BookService>(); // we registered the BookService class as the implementation for the IBookService interface in the dependency injection container. This means that whenever a component (like a controller) requests an IBookService, the framework will provide an instance of BookService. The AddScoped method indicates that the same instance will be used within a single request, but a new instance will be created for each new request.

            builder.Services.AddScoped<IAuthService,AuthService>();  

            var app = builder.Build();

            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            using (var scope = app.Services.CreateScope()) //When the application starts, create a temporary dependency injection scope, ask ASP.NET for a valid AppDbContext instance, then run the seeding logic using that database connection, then dispose everything.
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DataSeeder.SeedBooks(dbContext); // Seed initial data
            }

           // app.UseMiddleware<RequestIdMiddleware>();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseSerilogRequestLogging();

            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/", () =>
            {
                return Results.Redirect("/api/books");
            });

            app.Run();
        }
    }
}
