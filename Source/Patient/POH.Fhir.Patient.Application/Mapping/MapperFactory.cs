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

using POH.BusinessServices.Common.Abstractions;
using POH.BusinessServices.Common.Fhir.Abstractions.CodeSystemLookup;
using POH.BusinessServices.Common.Fhir.Abstractions.SecurityLabels;
using POH.BusinessServices.Common.Fhir.Abstractions.Translation;
using Demo.Fhir.Order.Application.Common.Constants;
using Demo.Fhir.Order.Application.Interfaces;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Profiles.UK;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Profiles.USCDI;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Shared;
using Demo.Fhir.Order.Application.Mapping.Profiles.UK;
using Demo.Fhir.Order.Application.Mapping.Profiles.USCDI;
using Demo.Fhir.Order.Application.Mapping.Shared;
using ExampleApp.Fhir.Common;
using ExampleApp.Fhir.Common.Exceptions;

namespace Demo.Fhir.Order.Application.Mapping
{
    /// <summary>
    /// Contains creating mapper logic
    /// </summary>
    public static class MapperFactory
    {
        /// <summary>
        /// Creates a profile-specific mapper orchestrator based on the requested FHIR profile.
        /// </summary>
        /// <param name="webApiHelper">WebAPI helper for environment configuration</param>
        /// <param name="ittLookup">Interface translation lookup</param>
        /// <param name="codeSystemLookup">Code system lookup</param>
        /// <param name="secLabelHelper">Security label helper</param>
        /// <param name="dataSource">Data source for order privacy lookup</param>
        /// <param name="profile">The FHIR profile to use (UKCore, USCore, or empty for auto-detection)</param>
        /// <returns>A mapper orchestrator that implements IDomainMapper for compile-time type safety</returns>
        public static IDomainMapper CreateMapper(IWebApiHelper webApiHelper,
            IInterfaceTranslation ittLookup,
            ICodeSystemLookup codeSystemLookup,
            ISecurityLabelHelper secLabelHelper,
            IDataSource dataSource,
            string profile)
        {            
            profile ??= string.Empty;

            if (string.IsNullOrEmpty(profile))
            {
                if (CountryProfileManager.GetCountryCode(webApiHelper).ToLower() == "gbr")
                {
                    profile = Constants.UKCoreProfile;
                }
            }

            switch (profile.ToLower())
            {
                case "ukcore":
                    return new MapperUK(
                        new MapContactUK(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource),
                        new MapOrderUK(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource),
                        new MapOrderProvenanceUK(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource),
                        new MapRedactedOrderUK(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource)
                    );

                case "uscore":
                case "":                
                    return new MapperUSCDI(
                        new MapContactUSCDI(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource),
                        new MapOrderUSCDI(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource),
                        new MapOrderProvenanceUSCDI(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource),
                        new MapRedactedOrderUSCDI(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource)
                    );

                default:
                    string message = $"ImplementationProfile '{profile}' is not supported.";
                    throw new FhirWebApiException(FhirErrorTypes.UnsupportedParameter, message);
            }
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