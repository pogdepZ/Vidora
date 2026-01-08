using AutoMapper;
using Vidora.Core.Entities;
using Vidora.Core.Contracts.Results;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;
using Vidora.Infrastructure.Api.Dtos.Responses;

namespace Vidora.Infrastructure.Api.Mapping;

public class StatsMappingProfile : Profile
{
    public StatsMappingProfile()
    {
        CreateMap<MovieData, Movie>();
        CreateMap<DashboardResponse, AdminDashboardResult>();
        CreateMap<UserData, User>();
    }
}
