using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vidora.Core.Entities;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;
using Vidora.Core.Contracts.Results;

namespace Vidora.Infrastructure.Api.Mapping
{
    public class StatsMappingProfile : Profile
    {
        public StatsMappingProfile()
        {
            CreateMap<MovieDto, Movie>();
            CreateMap<DashboardResponseDto, AdminDashboardResult>();
            CreateMap<UserDto, User>();
        }
    }
}
