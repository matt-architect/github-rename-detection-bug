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

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using POH.BusinessServices.Common.Abstractions;
using POH.BusinessServices.Common.Fhir.Abstractions.CodeSystemLookup;
using POH.BusinessServices.Common.Fhir.Abstractions.SecurityLabels;
using POH.BusinessServices.Common.Fhir.Abstractions.Translation;
using POH.BusinessServices.Common.Fhir.Extensions;
using Demo.Fhir.Order.Application.Interfaces;
using Demo.Fhir.Order.Application.Mapping.Shared;
using Demo.Fhir.Order.Domain.Entities;
using ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure.Interfaces;
using ExampleApp.Fhir.Common.Mdrx.V1.ReferenceTags;
using ExampleApp.Fhir.Common.Mdrx.V1.Resources;

namespace Demo.Fhir.Order.Application.Mapping.Profiles.UK
{
    using AMOrder = Domain.Entities.Order;

    /// <summary>
    /// Contains order profile for UKCore
    /// </summary>
    public class MapOrderUK : MapOrderBase
    {
        /// <summary>
        /// Prefix for General Practitioner
        /// </summary>
        public const string PrefixGeneralPractitioner = "GP";

        public MapOrderUK()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapOrderUK" /> class
        /// </summary>
        /// <param name="interfaceTranslation">
        /// Implementation of IInterfaceTranslation
        /// </param>
        /// <param name="codeSysLookup">
        /// Implementation of ICodeSystemLookup
        /// </param>
        /// <param name="securityLabelHelper">
        /// Implementation of ISecurityLabelHelper
        /// </param>
        /// <param name="webApiHelper">
        /// Implementation of IWebApiHelper
        /// </param>
        /// <param name="dataSource">
        /// Implementation of IDataSource
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        public MapOrderUK(IInterfaceTranslation interfaceTranslation, ICodeSystemLookup codeSysLookup, ISecurityLabelHelper securityLabelHelper, IWebApiHelper webApiHelper, IDataSource dataSource) : base(interfaceTranslation, codeSysLookup, securityLabelHelper, webApiHelper, dataSource)
        {
            // Example of mapping changing
            this.MappingExpression.ForMember(dest => dest.GeneralPractitioners,
                opt => opt.ResolveUsing(this.FhirGeneralPractitioner));
        }

        /// <summary>
        /// Get general practitioner
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Reference provider role</returns>
        protected override FhirList<Reference<IProviderRole>> FhirGeneralPractitioner(AMOrder order)
        {
            FhirList<Reference<IProviderRole>> generalPractitionerList = new FhirList<Reference<IProviderRole>>();
            order.ProviderList.ForEach(provider =>
            {
                if (!string.IsNullOrEmpty(provider.Identifier))
                {
                    var internalID = string.IsNullOrEmpty(provider.Identifier)
                        ? string.Empty
                        : $"{PrefixGeneralPractitioner}-{order.Identifier}-{provider.Identifier}";
                    Reference<IProviderRole> providerRole = null;

                    if (!provider.IsRedacted)
                    {
                        providerRole = new Reference<IProviderRole>
                        {
                            Ref = new Practitioner()
                            {
                                internalID = internalID,
                                SummaryDisplay = provider.Name?.LastName
                            }
                        };
                    }
                    else
                    {
                        // return redacted practitioner
                        providerRole = GetRedactedPractitioner(internalID);
                    }

                    generalPractitionerList.Add(providerRole);
                }
            });
            return generalPractitionerList;
        }

        /// <summary>
        /// Adds country specific extensions for identifiers
        /// </summary>
        /// <param name="idItem">
        /// Order identifier
        /// </param>
        /// <param name="order">
        /// Order data
        /// </param>
        /// <returns>
        /// List of country specific identifier extensions
        /// </returns>
        protected override List<IExtension> AddCountrySpecificExtensionsForIdentifiers(Id idItem, AMOrder order)
        {
            string nhsNumberEp = this.WebAPIHelper.GetEnvironmentProfile(
                "UKOrderHeaderNHSNumber",
                "Client Info",
                "");

            if (string.IsNullOrEmpty(nhsNumberEp) || string.IsNullOrEmpty(idItem.IDValue) ||
                !idItem.IDType.Equals(nhsNumberEp, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var verificationStatus = new ExampleApp.Fhir.Common.Mdrx.V1.Extensions.UKCORE.NHSNumberVerificationStatus
            {
                ExtensionValue = GetCodeableConceptVerificationStatus(order.NHSVerificationStatus)
            };

            return new List<IExtension> { verificationStatus };

        }

        /// <summary>
        /// Gets the Verification status extension
        /// </summary>
        /// <param name="orderNhsVerificationStatus"></param>
        /// <returns>Codeable concept for Verification status</returns>
        private CodeableConcept GetCodeableConceptVerificationStatus(string orderNhsVerificationStatus)
        {
            var codeableConcept = new CodeableConcept { Text = orderNhsVerificationStatus };
            var codingList = new List<Coding>();
            if (string.IsNullOrEmpty(orderNhsVerificationStatus))
            {
                var coding = new Coding
                {
                    Code = "NI",
                    Display = "NoInformation"
                };

                this.CodeSysLookup.TryLookupUri("NULLFL", out string systemUriNull);
                if (!string.IsNullOrEmpty(systemUriNull))
                {
                    coding.System = systemUriNull;
                }

                codingList.Add(coding);
                codeableConcept.Coding = codingList.Count > 0 ? codingList : null;
                codeableConcept.Text = "NoInformation";
                return codeableConcept;
            }

            var ancillaryStandardsEp = this.WebAPIHelper.GetEnvironmentProfile(
                "FHIR4_OrderNHSVerifStatusAncillaryCodeStandards",
                "DataXchg|Application Access API",
                "FHIR4_NHS");
           

            if (string.IsNullOrEmpty(ancillaryStandardsEp))
            {
                return codeableConcept;
            }
            DataTable verificationStatusDictionary = dataSource.FetchNHSVerificationStatusDictionary();
            foreach (DataRow row in verificationStatusDictionary.Rows)
            {
                if (row != null && !row.IsNull("Value") && orderNhsVerificationStatus.Equals(row["Value"].ToString(), StringComparison.OrdinalIgnoreCase) && !row.IsNull("CodingStandards") && ancillaryStandardsEp.Contains(row["CodingStandards"].ToString()!) && !row.IsNull("CodedValue"))
                {
                    AncillaryTranslation mapAncillaryTranslation = new AncillaryTranslation(row["CodedValue"].ToString());
                    if (mapAncillaryTranslation.HasValue)
                    {
                        var coding = new Coding();

                        coding.Code = mapAncillaryTranslation.Value;
                        if (!string.IsNullOrEmpty(mapAncillaryTranslation.Description))
                        {
                            coding.Display = mapAncillaryTranslation.Description;
                        }
                        else
                        {
                            coding.Display = !row.IsNull("Display") ? row["Display"].ToString() : orderNhsVerificationStatus;
                        }

                        if (!string.IsNullOrEmpty(mapAncillaryTranslation.CodingStandard))
                        {
                            this.CodeSysLookup.TryLookupUri(mapAncillaryTranslation.CodingStandard, out string systemUri);

                            if (!string.IsNullOrEmpty(systemUri))
                            {
                                coding.System = systemUri;
                            }
                            else if (AncillaryTranslation.IsValidSystemUri(mapAncillaryTranslation.CodingStandard))
                            {
                                coding.System = mapAncillaryTranslation.CodingStandard;
                            }
                            else
                            {
                                coding.System = this.InterfaceTranslation.TryGetMappedForExport("AncillaryCodingStd", mapAncillaryTranslation.CodingStandard, out IInterfaceTranslationMapped translated) ? translated.Value : null;
                            }
                        }

                        if (!string.IsNullOrEmpty(coding.Code) && !string.IsNullOrEmpty(coding.System))
                        {
                            codingList.Add(coding);
                        }
                    }
                }
            }

            codeableConcept.Coding = codingList.Count > 0 ? codingList : null;

            return codeableConcept;
        }

        /// <summary>
        /// Generate UK Profile Extensions
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fhirExtensionList"></param>
        public override void GenerateProfileExtensions(AMOrder source, List<IExtension> fhirExtensionList)
        {
            var orderBirthSexExtension = GetOrderBirthSexExtension(source);
            if (orderBirthSexExtension != null)
            {
                fhirExtensionList.Add(orderBirthSexExtension);
            }

            var orderEthnicCategoryExtension = GetOrderEthnicCategoryExtension(source);
            if (orderEthnicCategoryExtension != null)
            {
                fhirExtensionList.Add(orderEthnicCategoryExtension);
            }
        }

        /// <summary>
        /// Adds birth sex extension 
        /// </summary>
        /// <param name="source">
        /// Domain order object
        /// </param>
        private IExtension GetOrderBirthSexExtension(AMOrder source)
        {
            var birthSexCoding = FhirBirthSex(source);
            if (birthSexCoding == null)
            {
                return null;
            }

            var birthSexExt = new ExampleApp.Fhir.Common.Mdrx.V1.Extensions.UKCORE.BirthSex
            {
                ExtensionValue = birthSexCoding
            };

            return birthSexExt;
        }

        /// <summary>
        /// Adds birth sex extension 
        /// </summary>
        /// <param name="source">
        /// Domain order object
        /// </param>
        private IExtension GetOrderEthnicCategoryExtension(AMOrder source)
        {
            var ethnicity = FhirEthnicity(source);
            if (ethnicity == null)
            {
                return null;
            }

            var ethnicCoding = new List<Coding>();
            ethnicCoding.Add(ethnicity.Category);
            if (ethnicity.Details != null)
            {
                ethnicCoding.AddRange(ethnicity.Details);
            }

            var ethnicityExt = new ExampleApp.Fhir.Common.Mdrx.V1.Extensions.UKCORE.EthnicCategory
            {
                ExtensionValue = new CodeableConcept
                {
                    Coding = ethnicCoding,
                    Text = ethnicity.Text
                }
            };

            return ethnicityExt;
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
