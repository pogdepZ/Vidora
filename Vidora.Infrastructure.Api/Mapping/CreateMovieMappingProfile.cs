using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Entities;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Mapping
{
    public class CreateMovieMappingProfile : Profile
    {
        public CreateMovieMappingProfile()
        {
            // Map từ Response (API) sang Result (Core)
            // Nếu tên thuộc tính giống nhau (Id, Name) thì không cần .ForMember
            CreateMap<GenreResponseDto, GenreResult>();

            CreateMap<MemberResponseDto, MemberResult>();

            CreateMap<GenreDto, Genre>();
        }
    }
}
