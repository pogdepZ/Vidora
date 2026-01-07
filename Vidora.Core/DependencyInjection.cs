using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Mapping;
using Vidora.Core.Services;
using Vidora.Core.UseCases;

namespace Vidora.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Configure Core services here
        services.AddTransient<LoginUseCase>();
        services.AddTransient<AutoLoginUseCase>();
        services.AddTransient<LogoutUseCase>();
        services.AddTransient<RegisterUseCase>();
        services.AddTransient<GetDashboardStatsUseCase>();
        services.AddTransient<GetMoviesUseCase>();
        services.AddTransient<GetMovieDetailUseCase>();
        services.AddTransient<DeleteMovieUseCase>();
        services.AddTransient<CreateMovieUseCase>();
        services.AddTransient<UpdateMovieUseCase>();

        // User UseCases
        services.AddTransient<GetUsersUseCase>();
        services.AddTransient<GetUserDetailUseCase>();
        services.AddTransient<ToggleUserStatusUseCase>();

        // Subscription/Promo/Order UseCases
        services.AddTransient<GetSubscriptionPlansUseCase>();
        services.AddTransient<GetPromosUseCase>();
        services.AddTransient<CreatePromoUseCase>();
        services.AddTransient<GetOrdersUseCase>();

        // Services
        services.AddSingleton<ISessionStateService, SessionStateService>();
        services.AddTransient<IUserCredentialsService, UserCredentialsService>();

        // Mapping
        services.AddAutoMapper(typeof(LoginMappingProfile).Assembly);

        return services;
    }
}