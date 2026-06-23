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

using System.Collections.Generic;
using POH.BusinessServices.Common.Abstractions;
using POH.BusinessServices.Common.Fhir.Abstractions.CodeSystemLookup;
using POH.BusinessServices.Common.Fhir.Abstractions.SecurityLabels;
using POH.BusinessServices.Common.Fhir.Abstractions.Translation;
using POH.BusinessServices.Common.Fhir.Extensions;
using Demo.Fhir.Order.Application.Interfaces;
using ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements;

namespace Demo.Fhir.Order.Application.Mapping.Shared
{
    using AMOrder = Domain.Entities.Order;
    using FhirOrder = ExampleApp.Fhir.Common.Mdrx.V1.Resources.Order;

    /// <summary>
    /// Class for mapping the AM order to Fast Healthcare Interoperability Resource Order
    /// when order is redacted.
    /// </summary>
    public abstract class MapRedactedOrderBase : MapBase, IRedactedOrderMapper
    {
        public MapRedactedOrderBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapRedactedOrderBase"/> class.
        /// </summary>
        /// <param name="interfaceTranslation">Interface translation service</param>
        /// <param name="codeSysLookup">Code system lookup service</param>
        /// <param name="securityLabelHelper">Security label helper service</param>
        /// <param name="webAPIHelper">Web API helper service</param>
        /// <param name="dataSource">Data source service</param>
        public MapRedactedOrderBase(
            IInterfaceTranslation interfaceTranslation, 
            ICodeSystemLookup codeSysLookup, 
            ISecurityLabelHelper securityLabelHelper, 
            IWebApiHelper webAPIHelper, 
            IDataSource dataSource) 
            : base(interfaceTranslation, codeSysLookup, securityLabelHelper, webAPIHelper, dataSource)
        {
        }

        /// <summary>
        /// Maps a redacted AM Order to a FHIR Order resource.
        /// Only maps the internal ID and security tags for redacted orders.
        /// </summary>
        /// <param name="source">The source AM Order entity</param>
        /// <returns>A FHIR Order resource with only security-permitted fields</returns>
        public virtual FhirOrder Map(AMOrder source)
        {
            if (source == null)
            {
                return null;
            }

            var destination = new FhirOrder
            {
                internalID = source.Identifier.ToString(),
                SecurityAndPrivacyTags = GetSecurityTags(source)
            };

            return destination;
        }

        /// <summary>
        /// The GetSecurityTags method is used to map all the security tags for the resource.
        /// </summary>
        /// <param name="order">AM Order</param>
        /// <returns>List of security labels</returns>
        private List<Coding> GetSecurityTags(AMOrder order)
        {
            return this.SecurityLabelHelper.GetRedactedSecurityLabel();
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
