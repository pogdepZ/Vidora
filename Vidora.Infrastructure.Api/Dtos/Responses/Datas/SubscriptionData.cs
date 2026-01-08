using System;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

public record SubscriptionData(
    int SubscriptionId,
    string PlanName,
    DateTime startDate,
    DateTime EndDate,
    string Status
    );
