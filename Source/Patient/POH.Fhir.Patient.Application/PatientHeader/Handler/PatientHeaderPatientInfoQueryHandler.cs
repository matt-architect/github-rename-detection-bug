/* Copyright � 2025 - 2026 Example Corp. All Rights Reserved.
*
* L i c e n s e  N o t i c e: This software has been provided pursuant to a License Agreement,
with Example Corp, containing restrictions on its use. This
software contains valuable intellectual property and license information of Example Corp
and/or its subsidiaries and is protected by trade secret and copyright law. This software may not be
copied, reproduced, distributed, or publicly performed or displayed in any form or medium, used to
create derivative works, disclosed to any third parties, or used in any manner not provided for in
said License Agreement except with prior written authorization from Example Corp and/or
its subsidiaries. Notice to U.S. Government Users: This software is "Commercial Computer Software."

ExampleApp is a trademark of Example Corp.

*
**/
/* L i c e n s e  N o t i c e */

using FluentResults;
using POH.Infrastructure.Mediator;
using Demo.Fhir.Order.Application.Interfaces.Repositories.OrderHeader;
using Demo.Fhir.Order.Application.Mapping.OrderHeader;
using Demo.Fhir.Order.Application.OrderHeader.Models.Response;
using Demo.Fhir.Order.Application.OrderHeader.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Fhir.Order.Application.OrderHeader.Handler
{
    public class OrderHeaderOrderInfoQueryHandler : IRequestHandler<OrderHeaderOrderInfoQuery, Result<OrderHeaderOrderInfoResponse>>
    {

        private readonly IOrderHeaderMapper Mapper;
        private readonly IOrderHeaderRepository OrderHeaderRepository;

        public OrderHeaderOrderInfoQueryHandler(IOrderHeaderRepository orderHeaderRepository, IOrderHeaderMapper mapper)
        {
            OrderHeaderRepository = orderHeaderRepository;
            Mapper = mapper;
        }

        public async Task<Result<OrderHeaderOrderInfoResponse>> Handle(OrderHeaderOrderInfoQuery request, CancellationToken cancellationToken)
        {
            Result<OrderHeaderOrderInfoResponse> result;

            var repoResult = await OrderHeaderRepository.GetOrderHeaderOrderInfo(request.ClientGuid, request.VisitGuid, request.UserGuid);

            if (repoResult.IsSuccess)
            {
                var response = Mapper.MapOrderInfo(repoResult.Value);
                result = new Result<OrderHeaderOrderInfoResponse>().WithValue(response);
            }
            else
            {
                result = new Result<OrderHeaderOrderInfoResponse>().WithError($"Order Header Common Order Info Errors - {repoResult.Errors?[0]}");
            }

            return result;
        }

    }
}
/* L i c e n s e  N o t i c e */
/*
Confidential and license information of Example Corp. Authorized users only.
Notice to U.S. Government Users: This software is "Commercial Computer Software." Subject to full notice set
forth herein.
*/
/* L i c e n s e  N o t i c e */