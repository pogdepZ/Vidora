using AutoMapper;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;
using Vidora.Infrastructure.Api.Dtos.Requests;
using Vidora.Infrastructure.Api.Dtos.Responses;
using Vidora.Infrastructure.Api.Dtos.Responses.Metas;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Mapping;

public class SubscriptionMappingProfile : Profile
{
    public SubscriptionMappingProfile()
    {
        // Map SubscriptionPlanDto -> SubscriptionPlanResult
        CreateMap<SubscriptionPlanData, SubscriptionPlanResult>();

        // Map PromoItemDto -> PromoResult
        CreateMap<PromoData, PromoResult>();

        // Map PromoResponseDto -> PromoPaginationResult
        CreateMap<PromoResponse, PromoPaginationResult>()
            .ForCtorParam("Promos", opt => opt.MapFrom(src => src.Data))
            .ForCtorParam("Pagination", opt => opt.MapFrom(src => src.Pagination));

        // Map CreatePromoCommand -> CreatePromoRequestDto
        CreateMap<CreatePromoCommand, CreatePromoRequestDto>();

        // Map OrderItemDto -> OrderResult (flat structure)
        CreateMap<OrderData, OrderResult>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email));

        // Map OrderPaginationResponseDto -> OrderPaginationResult
        CreateMap<OrderPaginationResponseDto, OrderPaginationResult>()
            .ForCtorParam("Orders", opt => opt.MapFrom(src => src.Data))
            .ForCtorParam("Pagination", opt => opt.MapFrom(src => src.Pagination));

        // Map PaginationDto -> PaginationResult (if not already mapped elsewhere)
        CreateMap<PaginationMeta, PaginationResult>();
    }
}
