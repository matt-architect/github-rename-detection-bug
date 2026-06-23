/* L i c e n s e  N o t i c e */

/* Copyright © 2022-2025 Example Corp. All Rights Reserved.
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

namespace Demo.Fhir.Order.Application.Mapping.Shared
{
    using AMOrder = Domain.Entities.Order;
    using FhirOrder = ExampleApp.Fhir.Common.Mdrx.V1.Resources.Order;

    /// <summary>
    /// Interface for mapping redacted order data.
    /// </summary>
    public interface IRedactedOrderMapper
    {
        /// <summary>
        /// Maps a redacted AM Order to a FHIR Order resource.
        /// </summary>
        /// <param name="source">The source AM Order entity</param>
        /// <returns>A FHIR Order resource with only security-permitted fields</returns>
        FhirOrder Map(AMOrder source);
    }
}

/* L i c e n s e  N o t i c e */
/*
Confidential and license information of Example Corp. Authorized users only.
Notice to U.S. Government Users: This software is "Commercial Computer Software." Subject to full notice set
forth herein.
*/
/* L i c e n s e  N o t i c e */
