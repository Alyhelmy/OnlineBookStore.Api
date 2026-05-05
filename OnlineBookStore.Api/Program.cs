using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Shared.Data;
using OnlineBookStore.Api.Modules.Books.Interfaces;
using OnlineBookStore.Api.Modules.Books.Services;
using OnlineBookStore.Api.Shared.Middleware;
using Serilog;
using Serilog.Events;
using OnlineBookStore.Api.Modules.Auth.Interfaces;
using OnlineBookStore.Api.Modules.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OnlineBookStore.Api.Shared.Helpers;
using OnlineBookStore.Api.Modules.Orders.Interfaces;
using OnlineBookStore.Api.Modules.Orders.Services;


namespace OnlineBookStore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext() // we added the Enrich.FromLogContext() method to our Serilog configuration. This allows us to include contextual information (like the request ID) in our log entries by pushing properties into the log context during request processing.
                .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
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

            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters // this means that when a JWT token is received in an incoming request, the application will validate the token's issuer, audience, lifetime, and signing key against the specified parameters. If any of these validations fail, the token will be rejected and the request will be denied access to protected resources.
                    {
                        // switches to tell asp.net to validate the issuer, audience, lifetime, and signing key of incoming JWT tokens.

                        ValidateIssuer = true, 
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        //What exact value should ASP.NET compare against during that check?

                        ValidIssuer = builder.Configuration["Jwt:Issuer"], //here we are reading the expected issuer, audience, and signing key for JWT tokens from the application's configuration (e.g., appsettings.json). This allows us to ensure that only tokens issued by our trusted authority and intended for our application are accepted, and that they are properly signed to prevent tampering.
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(  
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            builder.Services.AddHttpContextAccessor(); // we added the AddHttpContextAccessor() method to the service collection. This registers the IHttpContextAccessor service, which allows us to access the current HTTP context (including request and response information) from anywhere in our application, such as in our custom middleware or services. This is particularly useful for logging contextual information like request IDs or user details in our logs

            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddCors(options =>  
            {
                options.AddPolicy("AllowAngularClient", policy =>
                {
                    policy.WithOrigins("http://localhost:4200",
                       "https://onlinebookstore.app" )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) 
            {
                app.MapOpenApi();
            }

            using (var scope = app.Services.CreateScope()) //When the application starts, create a temporary dependency injection scope, ask ASP.NET for a valid AppDbContext instance, then run the seeding logic using that database connection, then dispose everything.
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
                DataSeeder.SeedBooks(dbContext); // Seed initial data
            }

           // app.UseMiddleware<RequestIdMiddleware>();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseSerilogRequestLogging();

            app.UseCors("AllowAngularClient");

            app.UseAuthentication();

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
