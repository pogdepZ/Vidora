using AutoMapper;
using Vidora.Core.Contracts.Results;
using Vidora.Infrastructure.Api.Dtos.Responses;

namespace Vidora.Infrastructure.Mappings;

public class MovieDetailMappingProfile : Profile
{
    public MovieDetailMappingProfile()
    {
        // 1. Map thành viên (Actor/Director)
        CreateMap<MovieMemberDto, MovieMemberResult>();

        // 2. Map chi tiết phim
        CreateMap<MovieDetailDataDto, MovieDetailResult>()
            .ForCtorParam("MovieId", opt => opt.MapFrom(src => src.MovieId))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description))
            .ForCtorParam("ReleaseYear", opt => opt.MapFrom(src => src.ReleaseYear))
            .ForCtorParam("PosterUrl", opt => opt.MapFrom(src => src.PosterUrl))
            .ForCtorParam("BannerUrl", opt => opt.MapFrom(src => src.BannerUrl))
            .ForCtorParam("TrailerUrl", opt => opt.MapFrom(src => src.TrailerUrl))
            .ForCtorParam("MovieUrl", opt => opt.MapFrom(src => src.MovieUrl))
            // Ép kiểu sang double để tránh lỗi convert từ số nguyên 0 sang số thực
            .ForCtorParam("AvgRating", opt => opt.MapFrom(src => (double)src.AvgRating))
            .ForCtorParam("Genres", opt => opt.MapFrom(src => src.Genres))
            .ForCtorParam("Actors", opt => opt.MapFrom(src => src.Actors));
    }
}