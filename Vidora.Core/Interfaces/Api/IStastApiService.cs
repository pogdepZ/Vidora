using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;

namespace Vidora.Core.Interfaces.Api
{
    public interface IStatsApiService
    {
        Task<Result<AdminDashboardResult>> GetDashboardStatsAsync();
    }
}
