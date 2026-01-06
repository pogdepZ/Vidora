using AutoMapper;
using System.Collections.Generic;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Entities;
using Vidora.Infrastructure.Api.Dtos.Responses;

public class MoviePaginationMappingProfile : Profile
{
    public MoviePaginationMappingProfile()
    {
        // 1. Map phim (Id đều là string nên không cần cấu hình thêm)
        CreateMap<AdminMovieDto, AdminMovie>();

        // 2. Map Pagination (Quan trọng: Map TotalItems -> Total)
        CreateMap<PaginationDto, PaginationResult>()
    .ForCtorParam("Page", opt => opt.MapFrom(src => src.Page))
    .ForCtorParam("Limit", opt => opt.MapFrom(src => src.Limit))
    .ForCtorParam("Total", opt => opt.MapFrom(src => src.Total)) // Ép TotalItems vào Total
    .ForCtorParam("TotalPages", opt => opt.MapFrom(src => src.TotalPages));

        // 3. Map Object lớn chứa danh sách phim
        CreateMap<MoviePaginationResponseDto, MoviePaginationResult>()
            .ForCtorParam("Movies", opt => opt.MapFrom(src => src.Data))
            .ForCtorParam("Pagination", opt => opt.MapFrom(src => src.Pagination));
    }
}