using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;
using Vidora.Core.Contracts.Results;

namespace Vidora.Core.UseCases
{
    public class GetDashboardStatsUseCase
    {
        private readonly IStatsApiService _statsApi;
        private readonly ISessionStateService _sessionState;

        public GetDashboardStatsUseCase(IStatsApiService statsApi, ISessionStateService sessionState)
        {
            _statsApi = statsApi;
            _sessionState = sessionState;
        }

        public async Task<Result<AdminDashboardResult>> ExecuteAsync()
        {
            var token = _sessionState.CurrentSession?.AccessToken?.Token;
            if (string.IsNullOrEmpty(token)) return Result.Failure<AdminDashboardResult>("Unauthorized");

            return await _statsApi.GetDashboardStatsAsync();
        }
    }
}
