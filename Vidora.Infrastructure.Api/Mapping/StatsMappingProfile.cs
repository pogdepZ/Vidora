using AutoMapper;
using Vidora.Core.Entities;
using Vidora.Core.Contracts.Results;
using Vidora.Infrastructure.Api.Dtos.Responses;

namespace Vidora.Infrastructure.Api.Mapping;

public class StatsMappingProfile : Profile
{
    public StatsMappingProfile()
    {
        CreateMap<MovieDto, Movie>();
        CreateMap<DashboardResponse, AdminDashboardResult>();
        CreateMap<UserMovie, User>();
    }
}
