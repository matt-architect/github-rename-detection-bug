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
using System.Collections.Generic;
using System.Linq;

namespace Demo.Fhir.Order.Application.Mapping.OrderHeader
{
    /// <summary>
    /// Native mapper for OrderHeader entities to response DTOs.
    /// Replaces AutoMapper profiles for OrderHeaderOrderInfoProfile and SmallHeaderOrderInfoProfile.
    /// </summary>
    public class OrderHeaderMapper : IOrderHeaderMapper
    {
        /// <summary>
        /// Maps OrderHeaderOrderInfoEntity to full OrderHeaderOrderInfoResponse.
        /// </summary>
        /// <param name="entity">The order header order info entity</param>
        /// <returns>The order header order info response DTO</returns>
        public OrderHeaderOrderInfoResponse MapOrderInfo(OrderHeaderOrderInfoEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new OrderHeaderOrderInfoResponse
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                MiddleName = entity.MiddleName,
                Suffix = entity.Suffix,
                Gender = entity.Gender,
                MRN = entity.MRN,
                DisplayName = entity.DisplayName,
                PreferredName = entity.PreferredName,
                BirthDay = entity.BirthDay,
                BirthMonth = entity.BirthMonth,
                BirthYear = entity.BirthYear,
                ClientAge = entity.ClientAge,
                Pronouns = entity.Pronouns,
                ClientVisitNumber = entity.ClientVisitNumber,
                AgeDisplayValue = entity.AgeDisplayValue,
                ProviderDisplayName = entity.ProviderDisplayName,
                CurrentLocation = entity.CurrentLocation,
                TemporaryLocation = entity.TemporaryLocation,
                PrivacyStatusCode = entity.PrivacyStatusCode,
                SecurityLevel = entity.SecurityLevel,
                GenderIdentity = entity.GenderIdentity,
                BirthSex = entity.BirthSex,
                GenderLabel = entity.GenderLabel,
                FullImageEncoded = entity.FullImageEncoded,
                InternalVisitStatus = entity.InternalVisitStatus,
                VisitStatusDisplayName = entity.VisitStatusDisplayName,
                AdmittedDateAndTime = entity.AdmittedDateAndTime,
                AdmittedDate = entity.AdmittedDate,
                AdmittedTime = entity.AdmittedTime,
                DischargedDateAndTime = entity.DischargedDateAndTime,
                DischargedDate = entity.DischargedDate,
                DischargedTime = entity.DischargedTime,
                PlannedDischargedDateAndTime = entity.PlannedDischargedDateAndTime,
                PlannedDischargedDate = entity.PlannedDischargedDate,
                TimeZoneFriendlyName = entity.TimeZoneFriendlyName,
                TimeZone = entity.TimeZone,
                IsOrderBalanceEnabled = entity.IsOrderBalanceEnabled,
                FormattedPhone = entity.FormattedPhone,
                InsuranceCarrier = entity.InsuranceCarrier,
                InsurancePlan = entity.InsurancePlan,
                PolicyNumber = entity.PolicyNumber,
                HasInsuranceAccess = entity.HasInsuranceAccess,
                CanViewCanceledVisits = entity.CanViewCanceledVisits,
                CanMaintainDischargeData = entity.CanMaintainDischargeData,
                HasLocationAccess = entity.HasLocationAccess,
                IsVirtualEncounter = entity.IsVirtualEncounter,
                AllergySummary = MapAllergySummary(entity)
            };
        }

        /// <summary>
        /// Maps OrderHeaderOrderInfoEntity to SmallHeaderOrderInfoResponse.
        /// </summary>
        /// <param name="entity">The order header order info entity</param>
        /// <returns>The small header order info response DTO</returns>
        public SmallHeaderOrderInfoResponse MapSmallHeaderOrderInfo(OrderHeaderOrderInfoEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            // Direct property mapping - entity and response have matching property names
            return new SmallHeaderOrderInfoResponse
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                MiddleName = entity.MiddleName,
                Suffix = entity.Suffix,
                Gender = entity.Gender,
                MRN = entity.MRN,
                DisplayName = entity.DisplayName,
                PreferredName = entity.PreferredName,
                BirthDay = entity.BirthDay,
                BirthMonth = entity.BirthMonth,
                BirthYear = entity.BirthYear,
                ClientAge = entity.ClientAge,
                Pronouns = entity.Pronouns,
                ClientVisitNumber = entity.ClientVisitNumber,
                AgeDisplayValue = entity.AgeDisplayValue,
                CurrentLocation = entity.CurrentLocation,
                TemporaryLocation = entity.TemporaryLocation,
                PrivacyStatusCode = entity.PrivacyStatusCode,
                SecurityLevel = entity.SecurityLevel,
                GenderIdentity = entity.GenderIdentity,
                BirthSex = entity.BirthSex,
                GenderLabel = entity.GenderLabel,
                FullImageEncoded = entity.FullImageEncoded,
                AdmittedDateAndTime = entity.AdmittedDateAndTime,
                AdmittedDate = entity.AdmittedDate,
                AdmittedTime = entity.AdmittedTime,
                TimeZoneFriendlyName = entity.TimeZoneFriendlyName,
                TimeZone = entity.TimeZone
            };
        }

        /// <summary>
        /// Maps allergy information from OrderHeaderOrderInfoEntity to AllergySummary.
        /// </summary>
        /// <param name="entity">The order header order info entity</param>
        /// <returns>AllergySummary with allergy count and list</returns>
        private static AllergySummary MapAllergySummary(OrderHeaderOrderInfoEntity entity)
        {
            if (entity?.Allergies == null)
            {
                return new AllergySummary
                {
                    AllergyCount = 0,
                    Allergies = new List<AllergyItem>()
                };
            }

            return new AllergySummary
            {
                AllergyCount = entity.AllergyCount,
                Allergies = entity.Allergies.Select(a => new AllergyItem
                {
                    AllergenCode = a.AllergenCode,
                    AllergenType = a.AllergenType
                }).ToList()
            };
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
