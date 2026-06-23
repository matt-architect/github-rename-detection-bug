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

using System;
using System.Collections.Generic;
using AutoMapper;
using POH.BusinessServices.Common.Abstractions;
using POH.BusinessServices.Common.Fhir;
using POH.BusinessServices.Common.Fhir.Abstractions.CodeSystemLookup;
using POH.BusinessServices.Common.Fhir.Abstractions.SecurityLabels;
using POH.BusinessServices.Common.Fhir.Abstractions.Translation;
using POH.BusinessServices.Common.Fhir.Extensions;
using POH.BusinessServices.Common.Fhir.Mapping;
using POH.BusinessServices.Common.Fhir.ResourcesToInclude.Provenance;
using Demo.Fhir.Order.Application.Interfaces;
using ExampleApp.Fhir.Common;
using ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure;
using ExampleApp.Fhir.Common.Mdrx.V1.ReferenceTags;
using ExampleApp.Fhir.Common.Mdrx.V1.Resources;
using Provenance = Demo.Fhir.Order.Domain.Entities.Provenance;

namespace Demo.Fhir.Order.Application.Mapping.Shared
{
    using AMOrderProvenance = Provenance;
    using OrderV1 = ExampleApp.Fhir.Common.Mdrx.V1.Resources.Order;
    using POH.BusinessServices.Common.Fhir.Extensions;

    /// <summary>
    /// This class encapsulates all of the logic needed to map a Order <see cref="ExampleApp.Fhir.Common.Mdrx.V1.Resources.Provenance"/> record.
    /// </summary>
    public class MapOrderProvenanceBase : MapBase
    {
        protected readonly ProvenanceHelper provenanceHelper;

        /// <summary>
        /// Mapping expression
        /// </summary>
        protected IMappingExpression<AMOrderProvenance, ExampleApp.Fhir.Common.Mdrx.V1.Resources.Provenance> MappingExpression { get; private set; }

        public MapOrderProvenanceBase()
        {
        }

        /// <summary>
        /// Constructs an instance of <see cref="MapOrderProvenanceBase"/>
        /// </summary>
        /// <param name="interfaceTranslation">The concrete Interface Translator instance that implements <see cref="IInterfaceTranslation"/></param>
        /// <param name="codeSysLookup">The concrete Code System Lookup instance that implements <see cref="ICodeSystemLookup"/></param>
        /// <param name="securityLabelHelper">The concrete Security Label Helper instance that implements <see cref="ISecurityLabelHelper"/></param>
        /// <param name="webAPIHelper">The concrete WebAPI instance that implements <see cref="IWebApiHelper"/></param>
        /// <param name="dataSource">The concrete Data Source instance that implements <see cref="IDataSource"/></param>
        // ReSharper disable once InconsistentNaming
        public MapOrderProvenanceBase(IInterfaceTranslation interfaceTranslation, ICodeSystemLookup codeSysLookup, ISecurityLabelHelper securityLabelHelper, IWebApiHelper webAPIHelper, IDataSource dataSource) 
            : base(interfaceTranslation, codeSysLookup, securityLabelHelper, webAPIHelper, dataSource)
        {
            provenanceHelper = new ProvenanceHelper(webAPIHelper, codeSysLookup);

            this.MappingExpression = CreateMap<AMOrderProvenance, ExampleApp.Fhir.Common.Mdrx.V1.Resources.Provenance>();
            this.MappingExpression
                .IgnoreAllMembers() // Ignore All properties so that only mapped ones will be sent out
                .ForMember(dest => dest.internalID, opt => opt.MapFrom(src => GetInternalId(src.Identifier)))
                .ForMember(dest => dest.lastUpdated, opt => opt.MapFrom(src => GetLastUpdated(src.LastUpdatedWhen)))
                .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => GetLastActivity(src)))
                .ForMember(dest => dest.Agents, opt => opt.MapFrom(src => GetLastAgents(src)))
                .ForMember(dest => dest.Recorded, opt => opt.MapFrom(src => GetLastRecorded(src.LastUpdatedWhen)))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => GetLastTarget(src)));               
        }

        /// <summary>
        /// Returns the internal identifier of the parent order resource.
        /// </summary>
        /// <param name="internalId">The internal identifier of the parent order resource</param>
        /// <returns>The internal identifier of the parent order resource for provenance reference</returns>
        private static string GetInternalId(long internalId)
        {
            return $"{POHResourceConstant.Order}-{internalId}";
        }

        /// <summary>
        /// Returns the last updated <see cref="DateTimeOffset"/> for the target <see cref="OrderV1"/> resource.
        /// </summary>
        /// <param name="lastUpdatedWhen">The last updated <see cref="DateTimeOffset"/> for the parent order resource</param>
        /// <returns>The last updated <see cref="DateTimeOffset"/> for the target <see cref="OrderV1"/> resource</returns>
        private static DateTimeOffset? GetLastUpdated(DateTimeOffset? lastUpdatedWhen)
        {
            return lastUpdatedWhen;
        }

        /// <summary>
        /// Returns the last activity for the target <see cref="OrderV1"/> resource.
        /// </summary>
        /// <param name="provenance">The <see cref="AMOrderProvenance"/></param>
        /// <returns>A <see cref="Reference{OrderV1}"/></returns>
        private CodeableConcept GetLastActivity(AMOrderProvenance provenance)
        {
            if (provenance == null)
            {
                return null;
            }

            var isLastActivityCreate = provenance.TouchedWhen.Subtract(provenance.CreatedWhen).TotalSeconds <= 1;

            var coding = new Coding
            {
                Code = isLastActivityCreate ? "CREATE" : "UPDATE",
                Display = isLastActivityCreate ? "create" : "revise",
                System = CodeSysLookup.LookupUri("Data Operation Activity", "http://terminology.hl7.org/CodeSystem/v3-DataOperation")
            };

            var result = new CodeableConcept
            {
                Text = isLastActivityCreate ? "Order record created" : "Order record updated",
                Coding = new List<Coding> { coding }
            };

            return result;
        }

        /// <summary>
        /// Returns the last agents (author and transmitter) that touched the target <see cref="OrderV1"/> resource.
        /// </summary>
        /// <param name="provenance">The <see cref="AMOrderProvenance"/></param>
        /// <returns>A list of <see cref="Agent"/></returns>
        private List<Agent> GetLastAgents(AMOrderProvenance provenance)
        {
            var result = new List<Agent>();

            var authorAgent = GetLastAuthorAgent(provenance);
            if (authorAgent != null)
            {
                result.Add(authorAgent);
            }

            var transmitterAgent = GetLastTransmitterAgent(provenance);
            if (transmitterAgent != null)
            {
                result.Add(transmitterAgent);
            }

            return result;
        }

        /// <summary>
        /// Returns the last author <see cref="Agent"/>.
        /// </summary>
        /// <param name="provenance">The <see cref="AMOrderProvenance"/></param>
        /// <returns>The last author <see cref="Agent"/></returns>
        private Agent GetLastAuthorAgent(AMOrderProvenance provenance)
        {
            if (provenance == null)
            {
                return null;
            }

            IRequester who = CommonMappingHelper.MapToPractitioner(provenance.UserGuid.ToString(),
                DisplayHelper.BuildDisplayName(provenance.LastName, provenance.FirstName, provenance.MiddleName),
                provenance.OccupationCode);

            IRequester onBehalfOf = this.GetOrganization();

            return provenanceHelper.GetAgent(EnumAgentType.Author, who, onBehalfOf);

        }

        /// <summary>
        /// Returns the last transmitter <see cref="Agent"/>.
        /// </summary>
        /// <param name="provenance">The <see cref="AMOrderProvenance"/></param>
        /// <returns>The last transmitter <see cref="Agent"/></returns>
        private Agent GetLastTransmitterAgent(AMOrderProvenance provenance)
        {
            if (provenance == null)
            {
                return null;
            }

            IRequester who = this.GetOrganization();

            return provenanceHelper.GetAgent(EnumAgentType.Transmitter, who, null);
        }

        /// <summary>
        /// Get Managing Organization
        /// </summary>
        /// <returns>Return Managing Organization</returns>
        private Organization GetOrganization()
        {
            string sendingOrgName = this.WebAPIHelper.GetEnvironmentProfile(
                "SendingOrgName",
                "DataXchg",
                "SCM");

            // ReSharper disable once InconsistentNaming
            string localApplicationOID = this.WebAPIHelper.GetEnvironmentProfile(
                "LocalApplicationOID",
                "DataXchg",
                "SCM");

            Organization organization = new Organization()
            {
                SummaryDisplay = sendingOrgName,
                internalID = ManagingOrgantizationPrefix + localApplicationOID
            };

            return organization;
        }

        /// <summary>
        /// Returns the last recorded <see cref="FhirDateTimeOffset"/> for the target <see cref="OrderV1"/> resource.
        /// </summary>
        /// <param name="lastUpdatedWhen">The last recorded <see cref="DateTimeOffset"/> for the parent order resource</param>
        /// <returns>The last recorded <see cref="FhirDateTimeOffset"/> for the target <see cref="OrderV1"/> resource</returns>
        private static FhirDateTimeOffset GetLastRecorded(DateTimeOffset? lastUpdatedWhen)
        {
            var result = new FhirDateTimeOffset
            {
                DateTimeOffset = lastUpdatedWhen
            };
            return result;
        }

        /// <summary>
        /// Returns the last targeted <see cref="OrderV1"/> resource.
        /// </summary>
        /// <param name="provenance">The <see cref="AMOrderProvenance"/></param>
        /// <returns>A list of <see cref="Reference{OrderV1}"/></returns>
        private List<Reference<IAnyResource>> GetLastTarget(AMOrderProvenance provenance)
        {
            if (provenance == null)
            {
                return null;
            }

            var orderPrimaryName =
                CreateHumanNameFromEnterpriseName(provenance.TargetName, this.GetCoding("NameUse", "P", "NameUse"));

            var orderReference = new Reference<IAnyResource>
            {
                Ref = new OrderV1
                {
                    internalID = provenance.Identifier.ToString(),
                    SummaryDisplay = orderPrimaryName?.Text
                }
            };

            return new List<Reference<IAnyResource>> { orderReference };
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
