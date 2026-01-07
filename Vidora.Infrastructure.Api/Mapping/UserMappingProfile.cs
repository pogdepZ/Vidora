using AutoMapper;
using System.Linq;
using Vidora.Core.Contracts.Results;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

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
        CreateMap<UserPaginationResponseDto, UserPaginationResult>()
            .ForCtorParam("Users", opt => opt.MapFrom(src => src.Data))
            .ForCtorParam("Pagination", opt => opt.MapFrom(src => src.Pagination));

        // Map UserInfoDto -> AdminUserResult
        CreateMap<UserInfoDto, AdminUserResult>();

        // Map SubscriptionDto -> UserSubscriptionResult
        CreateMap<SubscriptionDto, UserSubscriptionResult>();

        // Map OrderDto -> UserOrderResult
        CreateMap<OrderDto, UserOrderResult>();

        // Map UserDetailDataDto -> UserDetailResult
        CreateMap<UserDetailDataDto, UserDetailResult>()
            .ForCtorParam("User", opt => opt.MapFrom(src => src.User))
            .ForCtorParam("Subscriptions", opt => opt.MapFrom(src => src.Subscriptions))
            .ForCtorParam("Orders", opt => opt.MapFrom(src => src.Orders));
    }
}
