// MIT License - Copyright (c) 2025-2026 Example Corp
using System;

namespace Example.Application.Mapping.OrderHeader
{
    public class OrderHeaderMapper : IOrderHeaderMapper
    {
        public OrderHeaderOrderInfoResponse MapOrderInfo(OrderHeaderOrderInfoEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return new OrderHeaderOrderInfoResponse
            {
                OrderId = entity.OrderIdentifier,
                Status = entity.OrderStatus,
                Priority = entity.OrderPriority
            };
        }

        public SmallHeaderOrderInfoResponse MapSmallHeaderOrderInfo(OrderHeaderOrderInfoEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return new SmallHeaderOrderInfoResponse { OrderId = entity.OrderIdentifier };
        }
    }
}