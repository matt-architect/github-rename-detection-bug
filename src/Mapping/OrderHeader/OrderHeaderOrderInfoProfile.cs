// MIT License
//
// Copyright (c) 2025 Example Corp. All Rights Reserved.
//
// This software has been provided pursuant to a License Agreement,
// with Example Corp, containing restrictions on its use. This
// software contains valuable trade secrets and proprietary information of
// Example Corp and is protected by trade secret and copyright law.
// This software may not be copied, reproduced, distributed, or publicly
// performed or displayed in any form or medium, used to create derivative
// works, disclosed to any third parties, or used in any manner not
// provided for in said License Agreement except with prior written
// authorization from Example Corp.
//
// Notice to U.S. Government Users: This software is "Commercial Computer Software."

using AutoMapper;
using Example.Application.OrderHeader.Models.Response;
using Example.Domain.Entities;

namespace Example.Application.Mapping.OrderHeader
{
    public class OrderHeaderOrderInfoProfile : Profile
    {
        public OrderHeaderOrderInfoProfile()
        {
            AllowNullCollections = true;
            AllowNullDestinationValues = true;

            CreateMap<OrderHeaderOrderInfoEntity, OrderHeaderOrderInfoResponse>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderIdentifier))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.OrderStatus))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.OrderPriority))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDateUtc))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDateUtc));
        }
    }
}

// Confidential and proprietary information of Example Corp. Authorized users only.
// Notice to U.S. Government Users: This software is "Commercial Computer Software."
// Subject to full notice set forth herein.