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

using AutoMapper;
using POH.BusinessServices.Common.Abstractions;
using POH.BusinessServices.Common.Fhir.Abstractions.CodeSystemLookup;
using POH.BusinessServices.Common.Fhir.Abstractions.SecurityLabels;
using POH.BusinessServices.Common.Fhir.Abstractions.Translation;
using POH.BusinessServices.Common.Fhir.Mapping;
using Demo.Fhir.Order.Application.Common.Constants;
using Demo.Fhir.Order.Application.Interfaces;
using Demo.Fhir.Order.Application.Mapping.Profiles.UK;
using Demo.Fhir.Order.Application.Mapping.Profiles.USCDI;
using Demo.Fhir.Order.Application.Mapping.Shared;
using ExampleApp.Fhir.Common;
using ExampleApp.Fhir.Common.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Demo.Fhir.Order.Application.Mapping
{
    /// <summary>
    /// Contains creating mapper logic
    /// </summary>
    public static class MapperFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="webApiHelper"></param>
        /// <param name="ittLookup"></param>
        /// <param name="codeSystemLookup"></param>
        /// <param name="secLabelHelper"></param>
        /// <param name="dataSource"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static IMapper CreateMapper(IWebApiHelper webApiHelper,
            IInterfaceTranslation ittLookup,
            ICodeSystemLookup codeSystemLookup,
            ISecurityLabelHelper secLabelHelper,
            IDataSource dataSource,
            string profile)
        {            
            MapperConfiguration config = null;

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
                    config = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile(new CommonMaps());
                        cfg.AddProfile(new MapContactUK(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource));
                        cfg.AddProfile(new MapOrderUK(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource));
                        cfg.AddProfile(new MapOrderProvenanceUK(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource));
                        cfg.AddProfile(new MapRedactedOrderUK(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource));
                    });
                    break;

                case "uscore":
                case "":                
                    config = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile(new CommonMaps());
                        cfg.AddProfile(new MapContactUSCDI(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource));
                        cfg.AddProfile(new MapOrderUSCDI(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource));
                        cfg.AddProfile(new MapOrderProvenanceUSCDI(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource));
                        cfg.AddProfile(new MapRedactedOrderUSCDI(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource));
                    });
                    break;

                default:
                    string message = $"ImplementationProfile '{profile}' is not supported.";
                    throw new FhirWebApiException(FhirErrorTypes.UnsupportedParameter, message);
            }

            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webApiHelper"></param>
        /// <param name="ittLookup"></param>
        /// <param name="codeSystemLookup"></param>
        /// <param name="secLabelHelper"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        //public static IMapper CreateRedactedMapper(
        //    IWebApiHelper webApiHelper,
        //    IInterfaceTranslation ittLookup,
        //    ICodeSystemLookup codeSystemLookup,
        //    ISecurityLabelHelper secLabelHelper,
        //    IDataSource dataSource)
        //{   
        //    MapperConfiguration config = new MapperConfiguration(cfg =>
        //    {
        //        cfg.AddProfile(new MapRedactedOrderBase(ittLookup, codeSystemLookup, secLabelHelper, webApiHelper, dataSource));
        //    });

        //    config.AssertConfigurationIsValid();
        //    return config.CreateMapper();
        //}
    }
}

/* L i c e n s e  N o t i c e */
/*
Confidential and license information of Example Corp. Authorized users only.
Notice to U.S. Government Users: This software is "Commercial Computer Software." Subject to full notice set
forth herein.
*/
/* L i c e n s e  N o t i c e */