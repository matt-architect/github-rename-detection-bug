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
using System.Linq;
using ExampleApp.Fhir.Common.Mdrx.V1.Resources;
using POH.BusinessServices.Common.Fhir;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Shared;
using Demo.Fhir.Order.Domain.Entities;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure;
using ExampleApp.Infrastructure.Paging;

namespace Demo.Fhir.Order.Application.FhirResult
{
    using EnterpriseOrder = Domain.Entities.Order;
    using OrderV1 = ExampleApp.Fhir.Common.Mdrx.V1.Resources.Order;
    using ProvenanceV1 = ExampleApp.Fhir.Common.Mdrx.V1.Resources.Provenance;
    using EnterpriseProvenance = Domain.Entities.Provenance;

    public class OrderFhirResultBuilder : POH.BusinessServices.Common.Fhir.FhirResultBuilder<OrderV1>
    {
        private readonly IEnumerable<EnterpriseOrder> enterpriseOrders;
        private readonly int numberOfResultsInRepository;
        private readonly IDomainMapper mapper;
        private readonly IDomainMapper redactedMapper;
        private bool isRedacted;

        /// <summary>
        /// Returns the total number of matching records found.
        /// </summary>
        public override int TotalNumberOfMatchingRecordsInDatabase => numberOfResultsInRepository;

        /// <summary>
        /// Fhir Resource Result to be sent back in the response.
        /// </summary>
        public override IEnumerable<OrderV1> ResourceResult
        {
            get
            {
                var mappedOrders = new List<OrderV1>();
                foreach (var enterpriseOrder in enterpriseOrders)
                {
                    if (enterpriseOrder.IsRedacted)
                    {
                        mappedOrders.Add(redactedMapper.Map<EnterpriseOrder, OrderV1>(enterpriseOrder, true));

                        // Setting global flag because whole order is redacted
                        this.isRedacted = true;
                    }
                    else
                    {
                        mappedOrders.Add(mapper.Map<EnterpriseOrder, OrderV1>(enterpriseOrder));

                        // scan order addresses and set global flag if any address is replaced by redaction extension
                        if (!this.isRedacted)
                        {
                            this.isRedacted = enterpriseOrder.AddressList != null &&
                                              enterpriseOrder.AddressList.Any(a => a.IsRedactionWarningRequired);
                        }

                        // scan order phones and set global flag if any phone is replaced by redaction extension
                        if (!this.isRedacted)
                        {
                            this.isRedacted = enterpriseOrder.PhoneList != null && enterpriseOrder.PhoneList
                                .Where(p => p.IsRedactionWarningRequired).Any();
                        }

                        // scan order identifiers and set global flag if any identifier is replaced by redaction extension
                        if (!this.isRedacted)
                        {
                            this.isRedacted = enterpriseOrder.IDList != null && enterpriseOrder.IDList
                                .Where(id => id.IsRedactionWarningRequired).Any();
                        }

                        // scan order contacts and set global flag if any contact is replaced by redaction extension
                        if (!this.isRedacted)
                        {
                            this.isRedacted = enterpriseOrder.ContactList != null && enterpriseOrder.ContactList
                                .Where(c => c.IsRedactionWarningRequired).Any();
                        }
                    }
                }

                return mappedOrders;
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{Provenance}"/> representing the list of Order Provenance Resources.
        /// </summary>
        public override IEnumerable<ProvenanceV1> Provenance
        {
            get
            {
                var mappedProvenances = new List<ProvenanceV1>();
                foreach (var order in enterpriseOrders)
                {
                    if (order.Provenance != null)
                    {
                        if (!order.IsRedacted)
                        {
                            mappedProvenances.Add(mapper.Map<EnterpriseProvenance, ProvenanceV1>(order.Provenance));
                        }
                    }
                }

                return mappedProvenances;
            }
        }

        /// <summary>
        /// Gets IsRedacted flag value.
        /// </summary>
        public override bool IsRedacted
        {
            get { return isRedacted; }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{FhirObject}"/> representing the list of resources to include in the result.
        /// </summary>
        /// <param name="resourceToInclude">The resource to include</param>
        /// <returns>An <see cref="IEnumerable{FhirObject}"/></returns>
        public override IEnumerable<FhirObject> GetResourceToInclude(string resourceToInclude)
        {
            return null;
        }

        /// <summary>
        /// Creates an instance of <see cref="OrderFhirResultBuilder"/>.
        /// </summary>
        /// <param name="totalNumberOfMatchingRecordsInDatabase">The total number of matching records</param>
        /// <param name="orders">An <see cref="IEnumerable{EnterpriseOrder}"/> representing the list of Order Resources.</param>
        /// <param name="mapper"></param>
        /// <param name="redactedMapper"></param>
        public OrderFhirResultBuilder(int totalNumberOfMatchingRecordsInDatabase,
            IEnumerable<EnterpriseOrder> orders, IDomainMapper mapper, IDomainMapper redactedMapper)
        {
            this.mapper = mapper;
            this.redactedMapper = redactedMapper;
            this.numberOfResultsInRepository = totalNumberOfMatchingRecordsInDatabase;
            this.enterpriseOrders = (orders != null) ? orders.ToList() : new List<EnterpriseOrder>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="extensions"></param>
        protected override void ApplyExtensionFiltering(IEnumerable<OrderV1> resources,
            IReadOnlyList<FreeFormExtension> extensions)
        {
            // do nothing

            // TO-DO: adding this change to get started with the other Extensions related PBIs.
            // It will be revisited later as part of other PBI's(-7981605) task.
        }
    }

    /// <summary>
    /// This class encapsulates all of the logic needed to build a <see cref="Orderv1"/> FHIR result.
    /// </summary>

    public class OrderFhirResultBuilderREDACT_WIP : FhirResultBuilder<OrderV1>
    {
        private readonly IEnumerable<EnterpriseOrder> enterpriseOrders;
        private readonly int numberOfResultsInRepository;
        private bool isRedacted;

        /// <summary>
        /// Returns the total number of matching records found.
        /// </summary>
        public override int TotalNumberOfMatchingRecordsInDatabase => numberOfResultsInRepository;


        /// <inheritdoc cref="FhirResultBuilder{T}.Build(List&lt;string&gt;, FhirCallContext, PagingInfoDetail, bool, IReadOnlyList&lt;FreeFormExtension&gt;)"/>
        public new FhirResult<OrderV1> Build(
            List<string> resourcesToInclude,
            FhirCallContext fhirCallContext,
            PagingInfoDetail pagingInfos,
            bool checkSummaryPropertiesForIndividualItem = false,
            IReadOnlyList<FreeFormExtension> extensions = null)
        {
            var noOrderResourcesToInclude = resourcesToInclude?.GetRange(0, resourcesToInclude.Count);
            noOrderResourcesToInclude?.RemoveAll(
                r => r.Equals("this.Order", StringComparison.OrdinalIgnoreCase));

            return base.Build(noOrderResourcesToInclude, fhirCallContext, pagingInfos,
                checkSummaryPropertiesForIndividualItem, extensions);
        }

        ///// <summary>
        ///// Fhir Resource Result to be sent back in the response.
        ///// </summary>
        //public override IEnumerable<Orderv1> ResourceResult
        //{
        //    get
        //    {
        //        var mappedOrders = new List<Orderv1>();
        //        foreach (var enterpriseOrder in enterpriseOrders)
        //        {
        //            if (enterpriseOrder.IsRedacted)
        //            {
        //                mappedOrders.Add(redactedMapper.Map<EnterpriseOrder, Orderv1>(enterpriseOrder));

        //                // Setting global flag because whole order is redacted
        //                this.isRedacted = true;
        //            }
        //            else
        //            {
        //                mappedOrders.Add(mapper.Map<EnterpriseOrder, Orderv1>(enterpriseOrder));

        //                // scan order addresses and set global flag if any address is replaced by redaction extension
        //                if (!this.isRedacted)
        //                {
        //                    this.isRedacted = enterpriseOrder.AddressList != null && enterpriseOrder.AddressList.Any(a => a.IsRedactionWarningRequired);
        //                }

        //                // scan order phones and set global flag if any phone is replaced by redaction extension
        //                if (!this.isRedacted)
        //                {
        //                    this.isRedacted = enterpriseOrder.PhoneList != null && enterpriseOrder.PhoneList.Where(p => p.IsRedactionWarningRequired).Any();
        //                }

        //                // scan order identifiers and set global flag if any identifier is replaced by redaction extension
        //                if (!this.isRedacted)
        //                {
        //                    this.isRedacted = enterpriseOrder.IDList != null && enterpriseOrder.IDList.Where(id => id.IsRedactionWarningRequired).Any();
        //                }

        //                // scan order contacts and set global flag if any contact is replaced by redaction extension
        //                if (!this.isRedacted)
        //                {
        //                    this.isRedacted  = enterpriseOrder.ContactList != null && enterpriseOrder.ContactList.Where(c => c.IsRedactionWarningRequired).Any();
        //                }
        //            }
        //        }

        //        return mappedOrders;
        //    }
        //}

        ///// <summary>
        ///// Returns an <see cref="IEnumerable{Provenance}"/> representing the list of Order Provenance Resources.
        ///// </summary>
        //public override IEnumerable<Provenancev1> Provenance
        //{
        //    get
        //    {
        //        var mappedProvenances = new List<Provenancev1>();
        //        foreach (var order in enterpriseOrders)
        //        {
        //            if (order.Provenance != null)
        //            {
        //                if (!order.IsRedacted)
        //                {
        //                    mappedProvenances.Add(mapper.Map<EnterpriseProvenance, Provenancev1>(order.Provenance));
        //                }
        //            }
        //        }

        //        return mappedProvenances;
        //    }
        //}

        /// <summary>
        /// Medication Request resource result.
        /// </summary>
        public override IEnumerable<OrderV1> ResourceResult { get; }

        /// <summary>
        /// Provenance resource result for Medication Request.
        /// </summary>
        public override IEnumerable<ProvenanceV1> Provenance { get; }

        /// <summary>
        /// Nothing to return.
        /// </summary>
        /// <param name="resourceToInclude"></param>
        /// <returns>A collection of Medication objects.</returns>
        public override IEnumerable<FhirObject> GetResourceToInclude(string resourceToInclude)
        {
            return null;
        }

        /// <summary>
        /// Gets IsRedacted flag value.
        /// </summary>
        public override bool IsRedacted { get; }

        ///// <summary>
        ///// Returns an <see cref="IEnumerable{FhirObject}"/> representing the list of resources to include in the result.
        ///// </summary>
        ///// <param name="resourceToInclude">The resource to include</param>
        ///// <returns>An <see cref="IEnumerable{FhirObject}"/></returns>
        //public override IEnumerable<FhirObject> GetResourceToInclude(string resourceToInclude)
        //{
        //    return null;
        //}

        /// <summary>
        /// Creates an instance of <see cref="OrderFhirResultBuilder"/>.
        /// </summary>
        /// <param name="totalNumberOfMatchingRecordsInDatabase">The total number of matching records</param>
        /// <param name="orders">An <see cref="IEnumerable{EnterpriseOrder}"/> representing the list of Order Resources.</param>
        /// <param name="isRedacted">A flag indicating if the order is redacted</param>
        public OrderFhirResultBuilderREDACT_WIP(int totalNumberOfMatchingRecordsInDatabase,
            IEnumerable<EnterpriseOrder> orders, bool isRedacted)
        {
            this.isRedacted = isRedacted;
            this.numberOfResultsInRepository = totalNumberOfMatchingRecordsInDatabase;
            this.enterpriseOrders = (orders != null) ? orders.ToList() : new List<EnterpriseOrder>();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="resources"></param>
        ///// <param name="extensions"></param>
        //protected override void ApplyExtensionFiltering(IEnumerable<Orderv1> resources, IReadOnlyList<FreeFormExtension> extensions)
        //{
        //    // do nothing

        //    // TO-DO: adding this change to get started with the other Extensions related PBIs.
        //    // It will be revisited later as part of other PBI's(-7981605) task.
        }
    }

/* L i c e n s e  N o t i c e */
/*
Confidential and license information of Example Corp. Authorized users only.
Notice to U.S. Government Users: This software is "Commercial Computer Software." Subject to full notice set
forth herein.
*/
/* L i c e n s e  N o t i c e */