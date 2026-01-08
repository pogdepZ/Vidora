﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System;
using Vidora.Core.Interfaces.Api;
using Vidora.Infrastructure.Api.Clients;
using Vidora.Infrastructure.Api.Handlers;
using Vidora.Infrastructure.Api.Mapping;
using Vidora.Infrastructure.Api.Options;
using Vidora.Infrastructure.Api.Services;

namespace Vidora.Infrastructure.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureApi(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Configure Infrastructure.Api services here
        // Services
        services.AddTransient<IAuthApiService, AuthApiService>();
        services.AddTransient<IStatsApiService, StatsApiService>();
        services.AddTransient<IMovieApiService, MovieApiService>();
        services.AddTransient<IUserApiService, UserApiService>();
        services.AddTransient<ISubscriptionApiService, SubscriptionApiService>();

        //
        services.AddTransient<AuthTokenHandler>();
        services.AddHttpClient<ApiClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.Timeout);
        })
        .AddHttpMessageHandler<AuthTokenHandler>();

        //
        services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);

        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.SectionName));
        return services;
    }
}
