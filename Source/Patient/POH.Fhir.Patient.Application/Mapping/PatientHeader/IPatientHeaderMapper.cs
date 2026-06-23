/* Copyright © 2025-2026 Example Corp. All Rights Reserved.
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

using Demo.Fhir.Order.Application.OrderHeader.Models.Response;
using Demo.Fhir.Order.Domain.Entities;

namespace Demo.Fhir.Order.Application.Mapping.OrderHeader
{
    /// <summary>
    /// Interface for mapping OrderHeader entities to response DTOs.
    /// </summary>
    public interface IOrderHeaderMapper
    {
        /// <summary>
        /// Maps OrderHeaderOrderInfoEntity to full OrderHeaderOrderInfoResponse.
        /// </summary>
        /// <param name="entity">The order header order info entity</param>
        /// <returns>The order header order info response DTO</returns>
        OrderHeaderOrderInfoResponse MapOrderInfo(OrderHeaderOrderInfoEntity entity);

        /// <summary>
        /// Maps OrderHeaderOrderInfoEntity to SmallHeaderOrderInfoResponse.
        /// </summary>
        /// <param name="entity">The order header order info entity</param>
        /// <returns>The small header order info response DTO</returns>
        SmallHeaderOrderInfoResponse MapSmallHeaderOrderInfo(OrderHeaderOrderInfoEntity entity);
    }
}

/* L i c e n s e  N o t i c e */
/*
Confidential and license information of Example Corp. Authorized users only.
Notice to U.S. Government Users: This software is "Commercial Computer Software." Subject to full notice set
forth herein.
*/
/* L i c e n s e  N o t i c e */
