using OnlineBookStore.Api.Modules.Admin.Interfaces;
using OnlineBookStore.Api.Modules.Admin.Services;
using OnlineBookStore.Api.Modules.Auth.Interfaces;
using OnlineBookStore.Api.Modules.Auth.Services;
using OnlineBookStore.Api.Modules.Books.Interfaces;
using OnlineBookStore.Api.Modules.Books.Services;
using OnlineBookStore.Api.Modules.Cart.Interfaces;
using OnlineBookStore.Api.Modules.Cart.Services;
using OnlineBookStore.Api.Modules.Orders.Interfaces;
using OnlineBookStore.Api.Modules.Orders.Services;
using OnlineBookStore.Api.Shared.Helpers;

namespace OnlineBookStore.Api.Shared.Extensions
{
    // Shared/Extensions/ServiceExtensions.cs
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            return services;
        }
    }
}
