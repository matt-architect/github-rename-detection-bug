/* L i c e n s e  N o t i c e */

/* Copyright © 2021-2025 Example Corp. All Rights Reserved.
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
    using AMProvenance = Domain.Entities.Provenance;
    using FhirProvenance = ExampleApp.Fhir.Common.Mdrx.V1.Resources.Provenance;

    /// <summary>
    /// Interface for mapping order provenance/audit information.
    /// </summary>
    public interface IProvenanceMapper
    {
        /// <summary>
        /// Maps AM Provenance to a FHIR Provenance resource.
        /// </summary>
        /// <param name="source">The source AM Provenance entity</param>
        /// <returns>A FHIR Provenance resource</returns>
        FhirProvenance Map(AMProvenance source);
    }
}

/* L i c e n s e  N o t i c e */
/*
Confidential and license information of Example Corp. Authorized users only.
Notice to U.S. Government Users: This software is "Commercial Computer Software." Subject to full notice set
forth herein.
*/
/* L i c e n s e  N o t i c e */
