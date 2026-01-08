using System;
using AutoMapper;
using System.Linq;
using Vidora.Core.Contracts.Results;
using Vidora.Infrastructure.Api.Dtos.Responses;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;
using Vidora.Infrastructure.Api.Dtos.Responses.Metas;

namespace Vidora.Infrastructure.Api.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Map UserData -> AdminUserResult
        CreateMap<UserData, AdminUserResult>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? string.Empty))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt ?? DateTime.MinValue));

        // Map PaginationMeta -> PaginationResult
        CreateMap<PaginationMeta, PaginationResult>()
            .ForCtorParam("Page", opt => opt.MapFrom(src => src.Page))
            .ForCtorParam("Limit", opt => opt.MapFrom(src => src.Limit))
            .ForCtorParam("Total", opt => opt.MapFrom(src => src.Total))
            .ForCtorParam("TotalPages", opt => opt.MapFrom(src => src.TotalPages));

        // Map UserPaginationResponse -> UserPaginationResult
        CreateMap<UserPaginationResponse, UserPaginationResult>()
            .ForCtorParam("Users", opt => opt.MapFrom(src => src.Data))
            .ForCtorParam("Pagination", opt => opt.MapFrom(src => src.Pagination));

        // Map SubscriptionData -> UserSubscriptionResult
        CreateMap<SubscriptionData, UserSubscriptionResult>()
            .ForCtorParam("SubscriptionId", opt => opt.MapFrom(src => src.SubscriptionId))
            .ForCtorParam("PlanName", opt => opt.MapFrom(src => src.PlanName))
            .ForCtorParam("StartDate", opt => opt.MapFrom(src => src.startDate))
            .ForCtorParam("EndDate", opt => opt.MapFrom(src => src.EndDate))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status));

        // Map OrderData -> UserOrderResult
        CreateMap<OrderData, UserOrderResult>()
            .ForCtorParam("OrderId", opt => opt.MapFrom(src => src.OrderId))
            .ForCtorParam("Amount", opt => opt.MapFrom(src => src.Amount))
            .ForCtorParam("PaymentMethod", opt => opt.MapFrom(src => src.PaymentMethodd))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt));

        // Map UserDetailData -> UserDetailResult
        CreateMap<UserDetailData, UserDetailResult>()
            .ForCtorParam("User", opt => opt.MapFrom(src => src.User))
            .ForCtorParam("Subscriptions", opt => opt.MapFrom(src => src.Subscriptions))
            .ForCtorParam("Orders", opt => opt.MapFrom(src => src.Orders));
    }
}
