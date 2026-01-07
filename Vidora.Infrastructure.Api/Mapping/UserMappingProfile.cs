using AutoMapper;
using System.Linq;
using Vidora.Core.Contracts.Results;
using Vidora.Infrastructure.Api.Dtos.Responses;

namespace Vidora.Infrastructure.Api.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Map UserItemDto -> AdminUserResult
        CreateMap<UserItemDto, AdminUserResult>();

        // Map PaginationDto -> PaginationResult
        CreateMap<PaginationDto, PaginationResult>();

        // Map UserPaginationResponseDto -> UserPaginationResult
        CreateMap<UserPaginationResponse, UserPaginationResult>()
            .ForCtorParam("Users", opt => opt.MapFrom(src => src.Data))
            .ForCtorParam("Pagination", opt => opt.MapFrom(src => src.Pagination));

        // Map UserInfoDto -> AdminUserResult
        CreateMap<UserInfoDto, AdminUserResult>();

        // Map SubscriptionDto -> UserSubscriptionResult
        CreateMap<SubscriptionData, UserSubscriptionResult>();

        // Map OrderDto -> UserOrderResult
        CreateMap<Dtos.Responses.OrderData, UserOrderResult>();

        // Map UserDetailDataDto -> UserDetailResult
        CreateMap<UserDetailData, UserDetailResult>()
            .ForCtorParam("User", opt => opt.MapFrom(src => src.User))
            .ForCtorParam("Subscriptions", opt => opt.MapFrom(src => src.Subscriptions))
            .ForCtorParam("Orders", opt => opt.MapFrom(src => src.Orders));
    }
}
