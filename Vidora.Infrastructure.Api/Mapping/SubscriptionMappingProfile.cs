using AutoMapper;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;
using Vidora.Infrastructure.Api.Dtos.Requests;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Mapping;

public class SubscriptionMappingProfile : Profile
{
    public SubscriptionMappingProfile()
    {
        // Map SubscriptionPlanDto -> SubscriptionPlanResult
        CreateMap<SubscriptionPlanDto, SubscriptionPlanResult>();

        // Map PromoItemDto -> PromoResult
        CreateMap<PromoItemDto, PromoResult>();

        // Map PromoResponseDto -> PromoPaginationResult
        CreateMap<PromoResponseDto, PromoPaginationResult>()
            .ForCtorParam("Promos", opt => opt.MapFrom(src => src.Data))
            .ForCtorParam("Pagination", opt => opt.MapFrom(src => src.Pagination));

        // Map CreatePromoCommand -> CreatePromoRequestDto
        CreateMap<CreatePromoCommand, CreatePromoRequestDto>();
    }
}
