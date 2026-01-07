using AutoMapper;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Entities;
using Vidora.Infrastructure.Api.Dtos.Responses;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;
using Vidora.Infrastructure.Api.Dtos.Responses.Metas;

namespace Vidora.Infrastructure.Api.Mapping;

public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        CreateMap<GenreResponse, GenreResult>();

        CreateMap<MembersResponseDto, MemberResult>();

        CreateMap<GenreData, Genre>();


        CreateMap<MemberData, MovieMemberResult>();

        CreateMap<MovieData, MovieDetailResult>()
            .ForCtorParam("MovieId", opt => opt.MapFrom(src => src.MovieId))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description))
            .ForCtorParam("ReleaseYear", opt => opt.MapFrom(src => src.ReleaseYear))
            .ForCtorParam("PosterUrl", opt => opt.MapFrom(src => src.PosterUrl))
            .ForCtorParam("BannerUrl", opt => opt.MapFrom(src => src.BannerUrl))
            .ForCtorParam("TrailerUrl", opt => opt.MapFrom(src => src.TrailerUrl))
            .ForCtorParam("MovieUrl", opt => opt.MapFrom(src => src.MovieUrl))
            .ForCtorParam("AvgRating", opt => opt.MapFrom(src => (double)src.AvgRating))
            .ForCtorParam("Genres", opt => opt.MapFrom(src => src.Genres))
            .ForCtorParam("Actors", opt => opt.MapFrom(src => src.Actors));


        //
        CreateMap<AdminMovieDto, AdminMovie>();

        CreateMap<PaginationMeta, PaginationResult>();

        CreateMap<MoviePaginationResponse, MoviePaginationResult>()
            .ForCtorParam("Movies", opt => opt.MapFrom(src => src.Data))
            .ForCtorParam("Pagination", opt => opt.MapFrom(src => src.Pagination));
    }
}
