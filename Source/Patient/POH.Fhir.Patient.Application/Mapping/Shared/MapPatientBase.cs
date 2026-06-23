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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using POH.BusinessServices.Common.Abstractions;
using POH.BusinessServices.Common.Fhir.Abstractions.CodedConcept;
using POH.BusinessServices.Common.Fhir.Abstractions.CodeSystemLookup;
using POH.BusinessServices.Common.Fhir.Abstractions.SecurityLabels;
using POH.BusinessServices.Common.Fhir.Abstractions.Translation;
using POH.BusinessServices.Common.Fhir.Extensions;
using Demo.Fhir.Order.Application.Common.Constants;
using Demo.Fhir.Order.Application.Interfaces;
using Demo.Fhir.Order.Application.Utils;
using Demo.Fhir.Order.Domain.Entities;
using Demo.Fhir.Order.Domain.Interfaces;
using ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements;
using ExampleApp.Fhir.Common.Mdrx.V1.ChoiceTags;
using ExampleApp.Fhir.Common.Mdrx.V1.Extensions;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure.Interfaces;
using ExampleApp.Fhir.Common.Mdrx.V1.ReferenceTags;
using ExampleApp.Fhir.Common.Mdrx.V1.Resources;
using Address = ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.Address;
using TribalAffiliation = ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.TribalAffiliation;

namespace Demo.Fhir.Order.Application.Mapping.Shared
{
    using AMOrder = Domain.Entities.Order;
    using IdClass = IdClass;
    using ProviderCategory = ProviderCategory;

    /// <summary>
    /// Class for mapping the AM order to Fast Healthcare Interoperability Resource Order.
    /// </summary>
    public class MapOrderBase : MapBase, IOrderMapper
    {
        /// <summary>
        /// Primary Phone Rank
        /// </summary>
        public const uint PrimaryPhoneRank = 1;

        /// <summary>
        /// Secondary Phone Rank
        /// </summary>
        public const uint SecondaryPhoneRank = 2;

        /// <summary>
        /// Prefix for free text provider id
        /// </summary>
        public const string PrefixForFreeTextProviderId = "FT_";

        

        /// <summary>
        /// System URI Birth Sex
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string SystemURIBirthSex = "BirthSex";        

        /// <summary>
        /// System URI Name Use.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string SystemURINameUse = "NameUse";

        /// <summary>
        /// System URI Common Language Code.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string SystemURICommonLanguageCode = "CommonLanguageCode";

        /// <summary>
        /// System URI Client ID Type Use.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string SystemURIClientIdTypeUse = "IdentifierUse";

        /// <summary>
        /// Security Label Criteria name for Order Privacy
        /// </summary>
        public const string CriteriaNameOrderPrivacy = "Order Privacy Status";

        /// <summary>
        /// Interface translation attribute for Language Code
        /// </summary>
        public const string IttAttributeLanguageCode = "LanguageCode";

        /// <summary>
        /// Security Label Criteria name for Order Identifier
        /// </summary>
        public const string CriteriaNameOrderIdentifier = "Order Identifier Type";

        /// <summary>
        /// Interface translation attribute for Language Ability
        /// </summary>
        public const string IttAttributeLanguageAbility = "LanguageAbility";

        /// <summary>
        /// System URI Common Language Ability.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string SystemURILanguageAbility = "LanguageAbilityMode";

        /// <summary>
        /// Privacy status lookup component
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected IDataSource dataSource;

        /// <summary>
        /// Contact mapper for contacts
        /// </summary>
        protected MapContactBase contactMapper;

        public MapOrderBase()
        {
        }
        /// <summary>
        ///   Initializes a new instance of the <see cref="MapOrderBase"/> class.
        /// </summary>
        /// <param name="interfaceTranslation">
        /// IInterfaceTranslation implementation
        /// </param>
        /// <param name="codeSysLookup">
        /// ICodeSystemLookup implementation
        /// </param>
        /// <param name="securityLabelHelper">
        /// ISecurityLabelHelper implementation
        /// </param>
        /// <param name="webAPIHelper">
        /// IWebApiHelper implementation
        /// </param>
        /// <param name="dataSource">
        /// IDataSource implementation
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        // ReSharper disable once InconsistentNaming
        public MapOrderBase(IInterfaceTranslation interfaceTranslation, ICodeSystemLookup codeSysLookup, ISecurityLabelHelper securityLabelHelper, IWebApiHelper webAPIHelper, IDataSource dataSource) :
            base(interfaceTranslation, codeSysLookup, securityLabelHelper, webAPIHelper, dataSource)
        {
            this.dataSource = dataSource;

            // Create a contact mapper instance to handle contact mapping
            this.contactMapper = new MapContactBase(interfaceTranslation, codeSysLookup, securityLabelHelper, webAPIHelper, dataSource);
        }

        /// <summary>
        /// Maps an AM Order to a FHIR Order resource.
        /// </summary>
        /// <param name="source">The source AM Order entity</param>
        /// <returns>A FHIR Order resource</returns>
        public virtual ExampleApp.Fhir.Common.Mdrx.V1.Resources.Order Map(AMOrder source)
        {
            if (source == null)
            {
                return null;
            }

            var addresses = this.FhirAddress(source);
            var contactPoints = this.FhirContactPoint(source);
            var contacts = this.AMContactList(source);
            var names = this.FhirName(source);
            var maritalStatus = this.FhirMaritalStatus(source);

            var destination = new ExampleApp.Fhir.Common.Mdrx.V1.Resources.Order
            {
                internalID = source.Identifier.ToString(),
                Identifiers = this.FhirIdentifier(source),
                Addresses = ConvertToFhirListAddress(addresses) ?? new FhirList<Address>(),
                BirthDate = this.FhirBirthDate(source),
                Communication = this.FhirCommunication(source),
                ContactPoints = ConvertToFhirListContactPoint(contactPoints) ?? new FhirList<ContactPoint>(),
                Contacts = contacts != null && contacts.Count > 0 ? ConvertContactsToFhirList(contacts) : null,
                Deceased = this.FhirDeceased(source),
                GeneralPractitioners = this.FhirGeneralPractitioner(source),
                IsActive = source.Active,
                ManagingOrganization = this.FhirManagingOrganization(),
                MaritalStatus = maritalStatus != null ? ConvertCodedConceptToCodeableConcept(maritalStatus) : null,
                Names = names != null && names.Count > 0 ? ConvertToFhirListHumanName(names) : null,
                MultipleBirth = this.FhirMultipleBirth(source),
                SecurityAndPrivacyTags = this.SecurityTags(source),
                lastUpdated = source.LastUpdatedWhen,
                Extensions = this.FhirExtensions(source)
            };

            // Conditional mappings based on redaction flags
            if (!source.IsSexRedacted)
            {
                destination.BirthGender = this.FhirBirthSex(source);
                destination.Gender = this.FhirGender(source);
            }

            if (!source.IsRaceRedacted)
            {
                destination.Race = this.FhirRace(source);
                destination.Ethnicity = this.FhirEthnicity(source);
            }

            // Conditional tribal affiliation
            var tribal = this.FhirTribalAffiliation(source);
            if (tribal != null && tribal.Any())
            {
                destination.TribalAffiliation = tribal;
            }

            // Conditional object version
            if (source.ObjectVersion != null)
            {
                destination.objectVersion = this.ObjectVersion(source.ObjectVersion);
            }

            AfterMap(source, destination);
            return destination;
        }

        /// <summary>
        /// Converts a List of Address to FhirList of Address.
        /// </summary>
        private FhirList<Address> ConvertToFhirListAddress(List<Address> source)
        {
            var result = new FhirList<Address>();

            if (source == null || source.Count == 0)
            {
                return result;
            }

            foreach (var item in source)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Converts a List of ContactPoint to FhirList of ContactPoint.
        /// </summary>
        private FhirList<ContactPoint> ConvertToFhirListContactPoint(List<ContactPoint> source)
        {
            var result = new FhirList<ContactPoint>();

            if (source == null || source.Count == 0)
            {
                return result;
            }

            foreach (var item in source)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Converts AM Contact list to FHIR OrderContact list using the contact mapper.
        /// </summary>
        private FhirList<OrderContact> ConvertContactsToFhirList(List<Domain.Entities.Contact> source)
        {
            if (source == null || source.Count == 0)
            {
                return null;
            }

            var result = new FhirList<OrderContact>();
            foreach (var contact in source)
            {
                var mappedContact = contactMapper.Map(contact);
                if (mappedContact != null)
                {
                    result.Add(mappedContact);
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a List of HumanName to FhirList of HumanName.
        /// </summary>
        private FhirList<HumanName> ConvertToFhirListHumanName(List<HumanName> source)
        {
            if (source == null || source.Count == 0)
            {
                return null;
            }

            var result = new FhirList<HumanName>();
            foreach (var item in source)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Converts POH CodedConcept to ExampleApp CodeableConcept.
        /// </summary>
        private CodeableConcept ConvertCodedConceptToCodeableConcept(CodedConcept source)
        {
            if (source == null)
            {
                return null;
            }

            return new CodeableConcept
            {
                Text = source.Text,
                Coding = source.Coding?.Select(code => new Coding
                {
                    Code = code.Value,
                    Display = code.Name,
                    System = code.System
                }).ToList()
            };
        }

        /// <summary>
        /// ObjectVersion
        /// </summary>
        /// <param name="objectVersion"></param>
        protected virtual string ObjectVersion(string objectVersion)
        {
            return null;
        }

        /// <summary>
        /// Get Managing Organization
        /// </summary>
        /// <returns>Return Managing Organization</returns>
        private Reference<Organization> FhirManagingOrganization()
        {
            Reference<Organization> managingOrganization = new Reference<Organization>();

            string sendingOrgName = this.WebAPIHelper.GetEnvironmentProfile(
                "SendingOrgName",
                "DataXchg",
                "SCM");

            // ReSharper disable once InconsistentNaming
            string localApplicationOID = this.WebAPIHelper.GetEnvironmentProfile(
                "LocalApplicationOID",
                "DataXchg",
                "SCM");

            managingOrganization.Ref = new Organization()
            {
                Name = sendingOrgName,
                internalID = ManagingOrgantizationPrefix + localApplicationOID
            };

            return managingOrganization;
        }

        /// <summary>
        /// Get Managing Organization
        /// </summary>
        /// <returns>Return Managing Organization</returns>
        public Reference<Organization> FhirOrganizationSummaryReference(string idType, IdClass idClass)
        {
            if (idClass != IdClass.Other)
            {
                return FhirManagingOrganization();
            }

            IInterfaceTranslationMapped translated;
            bool hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                "ClientIDTypeSystem",
                idType,
                out translated);

            if (hasItt && translated.HasValueAndDescription)
            {
                Reference<Organization> managingOrganization = new Reference<Organization>();
                managingOrganization.Ref = new Organization()
                {
                    SummaryDisplay = translated.Description
                };

                return managingOrganization;
            }

            return null;
        }

        /// <summary>
        /// Get marital status
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Marital status</returns>
        private CodedConcept FhirMaritalStatus(AMOrder order)
        {
            IInterfaceTranslationMapped translated;

            if (string.IsNullOrEmpty(order.MaritalStatus))
            {
                return null;
            }

            bool hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                "MaritalStatusCode",
                order.MaritalStatus,
                out translated);

            if (!hasItt)
            {
                // if <display> is not set, use untranslated value.
                // If<code> or<system> is not valued, do not populate fields in .coding.Set untranslated value in .text
                return new CodedConcept(order.MaritalStatus);
            }

            // if itt found.
            var codeableConcept = this.CodeSysLookup.LookupConcept(translated, order.MaritalStatus, order.MaritalStatus, "Marital Status");
            if (codeableConcept != null)
            {
                var codedConcept = new CodedConcept { Text = codeableConcept.Text };
                var coding = codeableConcept.Coding?.FirstOrDefault();
                if (coding != null)
                {
                    codedConcept.Coding = new List<Code>
                    {
                        new Code(coding.Code, coding.Display, coding.System)
                    };
                }

                return codedConcept;
            }

            return null;
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
        protected virtual List<IExtension> AddCountrySpecificExtensionsForIdentifiers(Id idItem, AMOrder order)
        {
            return null;
        }

        /// <summary>
        /// Get Identifier
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Return Identifier</returns>
        public virtual List<Identifier> FhirIdentifier(AMOrder order)
        {
            // order IdList contains duplicate Ids, so first get the unique Ids.
            var uniqueIdList = order.IDList.DistinctBy(id => new { id.IDType, id.IDValue, id.IDClass }).ToList();

            IInterfaceTranslationMapped translated;

            var identifierList = new List<Identifier>();
            foreach (var idItem in uniqueIdList)
            {
                // Let's check if mapping for id 
                var system = this.InterfaceTranslation.TryGetMappedForExport("ClientIDTypeSystem", idItem.IDType,
                    out translated)
                    ? translated.Value
                    : null;

                // Add identifier or redaction label only if ITT is configured
                if (!string.IsNullOrWhiteSpace(system))
                {
                    if (!idItem.IsRedacted)
                    {
                        var id = new Identifier()
                        {
                            Assigner = this.FhirOrganizationSummaryReference(idItem.IDType, idItem.IDClass),
                            Period = null,
                            System = system,
                            Type = this.GetCodeableConcept("ClientIDType", idItem.IDType),
                            Use = this.GetCoding("ClientIDTypeUse", idItem.IDType, SystemURIClientIdTypeUse, null,
                                false),
                            Value = idItem.IDValue,
                            ElementExtensions = AddCountrySpecificExtensionsForIdentifiers(idItem, order)
                        };
                        identifierList.Add(id);
                    }
                    else
                    {
                        // set warning is required flag because identifier is redacted in output
                        idItem.IsRedactionWarningRequired = true;
                        identifierList.Add(CreateRedactedElement<Identifier>());
                    }
                }
            }

            return identifierList.Count > 0 ? identifierList : null;
        }

        /// <summary>
        /// Get Redacted Identifier
        /// </summary>
        /// <param name="internalId"></param>
        /// <returns>Redacted Identifier</returns>
        protected virtual Identifier GetRedactedIdentifier(string internalId)
        {
            // todo: Innovation doesnt support the extension under Identifier yet. 
            // Revisit this code once the dependency is resolved.
            // var identifier = CreateRedactedElement<Identifier>();

            return null;
        }

        /// <summary>
        /// Get address
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Return address</returns>
        protected virtual List<Address> FhirAddress(AMOrder order)
        {
            List<Address> address = new List<Address>();

            var primaryAddress = this.CreateFhirAddressFromEnterpriseAddress(order.Address);
            if (primaryAddress != null)
            {
                address.Add(primaryAddress);
            }

            string primaryEmailType = this.WebAPIHelper.GetEnvironmentProfile(
                "PrimaryEmail",
                "Registration",
                string.Empty);

            foreach (var otherAddress in order.AddressList)
            {
                // exclude Primary Address as it is already added in the list
                // exclude the AddressType which is configured as PrimaryEmail
                if (otherAddress.Identifier != (order.Address == null ? 0 : order.Address.Identifier) && string.Compare(primaryEmailType, otherAddress.AddressType, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    var orderAddress = this.CreateFhirAddressFromEnterpriseAddress(otherAddress);
                    if (orderAddress != null)
                    {
                        address.Add(orderAddress);
                    }
                }
            }

            return address;
        }

        /// <summary>
        /// Get birth date
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Birth date</returns>
        private FhirDate FhirBirthDate(AMOrder order)
        {
            if (order.DateOfBirth == null)
            {
                return null;
            }

            FhirDate dob = null;

            if (order.DateOfBirth.DateTime.HasValue)
            {
                dob = new FhirDate();
                dob.DateTime = order.DateOfBirth.DateTime.Value.DateTime;
            }
            else
            {
                if (order.DateOfBirth.Year > 0)
                {
                    if (order.DateOfBirth.Month > 0)
                    {
                        dob = new FhirDate(string.Concat(order.DateOfBirth.Year, "-", order.DateOfBirth.Month.ToString("00")));
                    }
                    else
                    {
                        dob = new FhirDate(order.DateOfBirth.Year.ToString());
                    }
                }
            }

            // Add birth time extension
            // it is safe to call method AddOrderBirthTimeExtension
            // when dob is null or order.DateOfBirth is null
            AddOrderBirthTimeExtension(order.DateOfBirth, dob);
            return dob;
        }

        /// <summary>
        /// Get Birth Sex
        /// </summary>
        /// <param name = "order" > AM order</param>
        /// <returns>Return Birth Sex</returns>
        protected virtual Coding FhirBirthSex(AMOrder order)
        {
            if (string.IsNullOrEmpty(order.BirthSex))
            {
                return null;
            }

            return this.GetCoding("BirthSex", order.BirthSex, SystemURIBirthSex);
        }

        /// <summary>
        /// Get Communication
        /// </summary>
        /// <param name="order">AM Order</param>
        /// <returns>return Communication</returns>
        private List<Communication> FhirCommunication(AMOrder order)
        {
            List<Communication> communicationList = new List<Communication>();
           
            foreach (var prefLanguage in order.PreferredLanguageList)
            {
                IInterfaceTranslationMapped translated;

                var hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                    IttAttributeLanguageCode,
                    prefLanguage.Code,
                    out translated);

                var languageCodedConcept = this.GetCodedConcept(IttAttributeLanguageCode, prefLanguage.Code, SystemURICommonLanguageCode, translated);

                if (languageCodedConcept == null)
                {
                    continue;
                }

                var codingList = languageCodedConcept.Coding;

                var communicationItem = new Communication();
                switch (prefLanguage.PreferredType)
                {
                    case 0:
                        communicationItem.Preferred = false;
                        break;
                    case 1:
                        communicationItem.Preferred = true;
                        break;
                    default:
                        communicationItem.Preferred = null;
                            break;
                }

                communicationItem.Language = new CodeableConcept
                {
                    Coding = this.ConvertToFhirCoding(codingList),
                    Text = languageCodedConcept.Text
                };

                if (!string.IsNullOrEmpty(prefLanguage.Ability))
                {                     
                    var coding = this.GetCoding(IttAttributeLanguageAbility, prefLanguage.Ability, SystemURILanguageAbility, null, false);
                    if (coding != null)
                    {
                        var communicationProficiency = new ExampleApp.Fhir.Common.Mdrx.V1.Extensions.OrderProficiency();

                        communicationProficiency.orderProficiencyType.ExtensionValue = coding;
                        communicationItem.ElementExtensions ??= new List<IExtension>();
                        communicationItem.ElementExtensions.Add(communicationProficiency);
                    }
                }
                communicationList.Add(communicationItem);
            }

            return communicationList.Count > 0 ? communicationList : null;
        }

        /// <summary>
        /// Get Contact point
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Contact point</returns>
        private List<ContactPoint> FhirContactPoint(AMOrder order)
        {
            List<ContactPoint> contactPoint = new List<ContactPoint>();

            var primaryPhone = this.CreateContactPointFromEnterprisePhone(order.PrimaryPhone, PrimaryPhoneRank);
            if (primaryPhone != null)
            {
                contactPoint.Add(primaryPhone);
            }

            var secondaryPhone = this.CreateContactPointFromEnterprisePhone(order.SecondaryPhone, SecondaryPhoneRank);
            if (secondaryPhone != null)
            {
                contactPoint.Add(secondaryPhone);
            }

            foreach (var otherPhone in order.PhoneList)
            {
                // exclude Primary and Secondary Phone as it is already added in the list
                if (otherPhone.Identifier != order.PrimaryPhone?.Identifier && otherPhone.Identifier != order.SecondaryPhone?.Identifier)
                {
                    var phone = this.CreateContactPointFromEnterprisePhone(otherPhone, null);
                    if (phone != null)
                    {
                        contactPoint.Add(phone);
                    }
                }
            }

            var emailContact = this.GetPrimaryEmailContactPoint(order);
            if (emailContact != null)
            {
                contactPoint.Add(emailContact);
            }

            contactPoint.AddRange(GetConfidentialContactPoints(order));

            return contactPoint;
        }

        /// <summary>
        /// Get Primary Email ContactPoint
        /// </summary>
        /// <param name="order">AM Order</param>
        /// <returns>Primary Email ContactPoint</returns>
        private ContactPoint GetPrimaryEmailContactPoint(AMOrder order)
        {
            var primaryEmailType = this.WebAPIHelper.GetEnvironmentProfile(
                            "PrimaryEmail",
                            "Registration",
                            string.Empty);

            // find first address with type defined by EP PrimaryEmail
            var primaryEmailAddress = order.AddressList.Where(otherAddress =>
                    string.Compare(primaryEmailType, otherAddress.AddressType, StringComparison.OrdinalIgnoreCase) == 0)
                    .FirstOrDefault();

            // create contact point if primary email address found
            if (!string.IsNullOrEmpty(primaryEmailAddress?.AddressLine1))
            {
                return this.CreateContactPointFromEmailAddress(order, null, "Email");
            }

            // return null if primary email address is not found
            return null;
        }

        /// <summary>
        /// Creates contact points from confidential addresses if any
        /// </summary>
        /// <param name="order">AM Order</param>
        /// <returns>List of contact points</returns>
        private List<ContactPoint> GetConfidentialContactPoints(AMOrder order)
        {
            var confidentialContacts = new List<ContactPoint>();
            var confidentialAddressType = this.WebAPIHelper.GetEnvironmentProfile(
                "Confidential Visit Address",
                "Registration",
                string.Empty);

            if (!string.IsNullOrEmpty(confidentialAddressType))
            {
                foreach (var otherAddress in order.AddressList.Where(otherAddress =>
                    string.Compare(confidentialAddressType, otherAddress.AddressType,
                        StringComparison.OrdinalIgnoreCase) == 0))
                {                   
                    var contactPoint = this.CreateContactPointFromConfidentialAddress(otherAddress.AreaCode,
                        otherAddress.PhoneNumber, otherAddress.AddressType, otherAddress.IsRestricted, otherAddress.IsPrivate,
                        otherAddress.IsRedacted, otherAddress.VisitPrivacyStatusGUID);
                    if (contactPoint != null)
                    {
                        confidentialContacts.Add(contactPoint);
                    }                   
                }
            }

            return confidentialContacts;
        }

        /// <summary>
        /// Creates contact point from phone and address type
        /// </summary>
        /// <param name="areaCode">
        /// Phone area code
        /// </param>
        /// <param name="phoneNumber">
        /// Phone number
        /// </param>
        /// <param name="addressType">
        /// Address type
        /// </param>
        /// <param name="isRestricted">
        /// Value indicating whether address is associated with confidential restricted visit
        /// </param>
        /// <param name="isPrivate">
        /// Value indicating whether address is associated with confidential private visit
        /// </param>
        /// <param name="isRedacted">
        /// Value indicating whether address is redacted
        /// </param>
        /// <param name="visitPrivacyStatusGuid">
        /// Value indicating the value of visit privacy status guid
        /// </param>
        /// <returns>
        /// Returns contact point if data provided and translation is configured, null otherwise
        /// </returns>
        private ContactPoint CreateContactPointFromConfidentialAddress(string areaCode, string phoneNumber, string addressType, bool isRestricted, bool isPrivate, bool isRedacted, long visitPrivacyStatusGuid)
        {
            var attributeName = "ConfidentialPhoneUse";
            if (string.IsNullOrEmpty(areaCode) || string.IsNullOrEmpty(phoneNumber))
            {
                return null;
            }

            var hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                attributeName,
                addressType,
                out IInterfaceTranslationMapped translated);

            if (!hasItt)
            {
                return null;
            }

            if (isRedacted)
            {
                return CreateRedactedElement<ContactPoint>();
            }

            return new ContactPoint()
            {
                Value = $"({areaCode.Trim()}) {phoneNumber.Trim()}",
                Period = null,
                Rank = null,
                System = new Coding { Code="phone", Display = "Phone", System = "http://hl7.org/fhir/contact-point-system" },
                Use = this.GetCoding(attributeName, addressType, SystemURIContactPointUse, translated),
                ElementExtensions = EncounterBasedSecurityTags<ContactPoint>(isRestricted, isPrivate, visitPrivacyStatusGuid)
            };
        }

        /// <summary>
        /// Get AM Order Contact List
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>AM Order Contact List</returns>
        // ReSharper disable once InconsistentNaming
        private List<Contact> AMContactList(AMOrder order)
        {
            // Ignore contact if both ITTs 'ContactTypeCode' and 'RelationshipCode' are not configured
            List<Contact> contactList = new List<Contact>();
            foreach (var contact in order.ContactList)
            {
                var hasContactTypeCodeItt = false;

                if (!string.IsNullOrWhiteSpace(contact.ContactType))
                {
                    hasContactTypeCodeItt = this.InterfaceTranslation.TryGetMappedForExport(
                        "ContactTypeCode",
                        contact.ContactType,
                        out IInterfaceTranslationMapped _);
                }

                var hasRelationshipCodeItt = false;

                if (!string.IsNullOrWhiteSpace(contact.RelationshipType))
                {
                    hasRelationshipCodeItt = this.InterfaceTranslation.TryGetMappedForExport(
                        "RelationshipCode",
                        contact.RelationshipType,
                        out IInterfaceTranslationMapped _);
                }

                if (hasContactTypeCodeItt || hasRelationshipCodeItt)
                {
                    contactList.Add(contact);
                }
            }

            return contactList.Count > 0 ? contactList : null;
        }

        /// <summary>
        /// Get deceased data
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Deceased info</returns>
        private IDeceasedChoice FhirDeceased(AMOrder order)
        {
            if (order.DeceasedDtmOffset.HasValue)
            {
                return new FhirDateTimeOffset()
                {
                    DateTimeOffset = order.DeceasedDtmOffset.Value,
                };
            }
            if (order.DeceasedDtm.HasValue)
            {
                return new FhirDateTimeOffset()
                {                    
                    PartialDate = $"{order.DeceasedDtm.Value.Year}-{order.DeceasedDtm.Value.Month.ToString("00")}-{order.DeceasedDtm.Value.Day.ToString("00")}"
                };
            }

            return new FhirBool(false);
        }

        /// <summary>
        /// Get gender
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Return Gender</returns>
        private Coding FhirGender(AMOrder order)
        {
            Coding genderCoding;

            genderCoding = this.GetCoding("GenderCode", order.Gender, Constants.SystemURIAdministrativeGender);

            if (genderCoding == null)
            {
                // Gender element must be present in output
                // So, we return coding containing Administrative gender and order gender as is when ITT is configured without code component
                this.CodeSysLookup.TryLookupUri(Constants.SystemURIAdministrativeGender, out string defaultSystemUri);
                genderCoding = new Coding(defaultSystemUri, order.Gender);

                // Set display to empty string to keep display element in output
                genderCoding.Display = string.Empty;
            }

            return genderCoding;
        }

        /// <summary>
        /// Get general practitioner
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Reference provider role</returns>
        protected virtual FhirList<Reference<IProviderRole>> FhirGeneralPractitioner(AMOrder order)
        {
            FhirList<Reference<IProviderRole>> generalPractitionerList = new FhirList<Reference<IProviderRole>>();
                        
            order.ProviderList.ForEach(provider =>
            {
                if (provider.Name != null)
                {
                    var internalID = provider.ProviderCategory == ProviderCategory.FreeText
                        ? $"{PrefixForFreeTextProviderId}{provider.Identifier}"
                        : $"{provider.Identifier}";
                    Reference<IProviderRole> providerRole = null;

                    if (!provider.IsRedacted)
                    {
                        providerRole = new Reference<IProviderRole>
                        {
                            Ref = new Practitioner()
                            {
                                internalID = internalID,
                                SummaryDisplay = provider.ProviderCategory == ProviderCategory.FreeText
                                    ? $"{provider.Name.LastName}, {provider.Name.FirstName} {provider.Name.MiddleName}"
                                        .Trim().TrimEnd(',')
                                    : $"{provider.Name.LastName}, {provider.Name.FirstName} {provider.Name.MiddleName}"
                                        .Trim().TrimEnd(',') + $" ({provider.Occupation})"
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
        /// Get Redacted Practitioner
        /// </summary>
        /// <param name="internalId"></param>
        /// <returns>Redacted Practitioner</returns>
        protected virtual Reference<IProviderRole> GetRedactedPractitioner(string internalId)
        {
            var practitioner = CreateRedactedElement<Practitioner>();
            practitioner.internalID = internalId;

            return new Reference<IProviderRole>
            {
                Ref = practitioner
            };
        }

        /// <summary>
        /// Get Human names
        /// </summary>
        /// <param name="order">AM order</param>
        /// <returns>Human name list</returns>
        private List<HumanName> FhirName(AMOrder order)
        {
            List<HumanName> nameList = new List<HumanName>();

            var primaryName = this.CreateHumanNameFromEnterpriseName(order.Name, this.GetCoding("NameUse", "P", SystemURINameUse));
            if (primaryName != null)
            {
                nameList.Add(primaryName);
            }

            foreach (var otherName in order.OtherNameList)
            {
                var name = this.CreateHumanNameFromEnterpriseName(otherName, this.GetCoding("NameUse", otherName.NameType, SystemURINameUse));
                if (name != null)
                {
                    nameList.Add(name);
                }
            }

            return nameList;
        }

        /// <summary>
        /// Resolves multiple birth 
        /// </summary>
        /// <param name="order">AM order data</param>
        /// <returns>MultipleBirth value</returns>
        private FhirBool FhirMultipleBirth(AMOrder order)
        {
            FhirBool retVal = new FhirBool(false);
            retVal.Value = order.IsMultipleBirth;
            return retVal;
        }

        /// <summary>
        /// Finalize object mapping
        /// </summary>
        /// <param name="order">ExampleApp order object</param>
        /// <param name="adapterOrder">FHIR order object</param>
        protected virtual void AfterMap(AMOrder order, ExampleApp.Fhir.Common.Mdrx.V1.Resources.Order adapterOrder)
        {
            adapterOrder.SummaryDisplay = this.SummaryDisplay(order, adapterOrder);

            // Do I need to nullify empty element extension list?
            // removing empty contact extension list
            if (adapterOrder.Contacts == null)
            {
                return;
            }

            foreach (var contact in adapterOrder.Contacts)
            {
                if (contact.ElementExtensions != null && contact.ElementExtensions.Count == 0)
                {
                    contact.ElementExtensions = null;
                }
            }
        }

        /// <summary>
        /// Generate summary display
        /// </summary>
        /// <param name="order">AM Order object</param>
        /// <param name="adapterOrder">Adapter order object</param>
        /// <returns>summary display text</returns>
        private string SummaryDisplay(AMOrder order, ExampleApp.Fhir.Common.Mdrx.V1.Resources.Order adapterOrder)
        {
            if (adapterOrder == null)
            {
                return string.Empty;
            }

            // ReSharper disable once InconsistentNaming
            string dateFormatFromEP = this.WebAPIHelper.GetEnvironmentProfile(
               "CompositeDateFormat",
               "Connect",
               "dd-MMM-yyyy");

            // ReSharper disable once InconsistentNaming
            string partialDateFormatFromEP = this.WebAPIHelper.GetEnvironmentProfile(
               "PartialCompositeDateFormat",
               "Connect",
               "MMM yyyy");

            string primaryEmailType = this.WebAPIHelper.GetEnvironmentProfile(
                "PrimaryEmail",
                "Registration",
                string.Empty);

            var orderPrimaryAddress = this.CreateFhirAddressFromEnterpriseAddress(order.Address);
            var orderPrimaryName = this.CreateHumanNameFromEnterpriseName(order.Name, this.GetCoding("NameUse", "P", SystemURINameUse));
            var orderPrimaryEmail = this.GetPrimaryEmailContactPoint(order);
            
            System.Text.StringBuilder addresses = new System.Text.StringBuilder();
            System.Text.StringBuilder phones = new System.Text.StringBuilder();
           
            Dictionary<string, string> propertyList = new Dictionary<string, string>();
        
            propertyList.Add("name", orderPrimaryName != null  ? orderPrimaryName.Text : string.Empty);
            
            if (adapterOrder.BirthDate != null)
            {
                if (adapterOrder.BirthDate.DateTime != null)
                {
                    propertyList.Add("Date of Birth", adapterOrder.BirthDate.DateTime.Value.ToString(dateFormatFromEP));
                }
                else if (!string.IsNullOrEmpty(adapterOrder.BirthDate.PartialDate))
                {
                    if (order.DateOfBirth.Year > 0)
                    {
                        string strPartialDate = PartialDateUtils.FormatPartialDate(order.DateOfBirth.Year, order.DateOfBirth.Month, order.DateOfBirth.Day, partialDateFormatFromEP);
                        if (!string.IsNullOrEmpty(strPartialDate))
                        {
                            propertyList.Add("Date of Birth", strPartialDate);
                        }
                    }                   
                }                
            }

            if (adapterOrder.Gender != null && !string.IsNullOrEmpty(adapterOrder.Gender.Code))
            {
                propertyList.Add("Gender", adapterOrder.Gender.Code);
            }

            System.Text.StringBuilder retValue = new System.Text.StringBuilder();
            foreach (var item in propertyList)
            {
                if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                {
                    if (item.Key.Equals("name", StringComparison.OrdinalIgnoreCase))
                    { 
                        retValue.Append($"{ item.Value } | "); 
                    }
                    else 
                    {
                        retValue.Append($"{ item.Key }: { item.Value } | ");
                    }
                }
            }
          
            if (adapterOrder.Addresses.Count > 0)
            {
                // Adapter address includes Address status = "Inactive", but only Address with Active status should be displayed
                // Assuming primary address is always order scope and cannot be redacted
                if (order.Address!= null && order.Address.Status.Equals("Active", StringComparison.OrdinalIgnoreCase) && orderPrimaryAddress != null && !string.IsNullOrEmpty(orderPrimaryAddress.Text) && orderPrimaryAddress.Use != null)
                {
                    addresses.Append(string.Concat("Address (", orderPrimaryAddress.Use.Code, "): ", orderPrimaryAddress.Text, " | "));
                }

                var activeAddresslist = order.AddressList.Where(otherAddress => string.Compare(primaryEmailType, otherAddress.AddressType, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare("Active", otherAddress.Status, StringComparison.OrdinalIgnoreCase) == 0).ToList();
                foreach (var otherAddress in activeAddresslist)
                {
                    // Create Fhir address for "Active" status only
                    var item = this.CreateFhirAddressFromEnterpriseAddress(otherAddress);
                    if (item != null && !string.IsNullOrEmpty(item.Text) && item.Use != null &&
                        !string.IsNullOrEmpty(item.Use.Code))
                    {
                        addresses.Append(string.Concat("Address (", item.Use.Code, "): ", item.Text, " | "));
                    }
                }
            }

            retValue.Append(addresses);

            if (adapterOrder.ContactPoints.Count > 0)
            {
                foreach (var item in adapterOrder.ContactPoints)
                {
                    if (orderPrimaryEmail != null && item != null && !string.IsNullOrEmpty(item.Value) &&
                        string.Equals(item.Value, orderPrimaryEmail.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        phones.Append(string.Concat("Email: ", item.Value, " | "));
                    }
                    else if (item != null && !string.IsNullOrEmpty(item.Value) && item.Use != null &&
                             !string.IsNullOrEmpty(item.Use.Code))
                    {
                        phones.Append(string.Concat("Phone (", item.Use.Code, "): ", item.Value, " | "));
                    }
                }
            }

            retValue.Append(phones);
            return retValue.ToString().Trim().TrimEnd('|').Trim();
        }

        /// <summary>
        /// The SecurityTags method is used to map all the security tags for the resource.
        /// </summary>
        /// <param name="order">AM Order</param>
        /// <returns>List of security labels</returns>
        private List<Coding> SecurityTags(AMOrder order)
        {
            List<Coding> tags = new List<Coding>();
            long? privacyStatusGuid = this.FindPrivacyStatusGuidByCode(order.PrivacyStatus);

            this.SecurityLabelHelper.GetSecurityLabel(tags, CriteriaNameOrderPrivacy, privacyStatusGuid);

            if (order.IDList != null)
            {
                foreach (var idItem in order.IDList)
                {
                    // Add security label only if ITT "ClientIDTypeSystem" is configured for this identifier type
                    if (this.InterfaceTranslation.TryGetMappedForExport("ClientIDTypeSystem", idItem.IDType,
                            out IInterfaceTranslationMapped _))
                    {
                        if (!string.IsNullOrEmpty(idItem.IDValue) && idItem.IDTypeIdentifier > 0)
                        {
                            this.SecurityLabelHelper.GetSecurityLabel(tags, CriteriaNameOrderIdentifier,
                                idItem.IDTypeIdentifier);
                        }
                    }
                }
            }

            return tags;
        }

        /// <summary>
        /// Looks up for privacy status code 
        /// </summary>
        /// <param name="code">Privacy status code</param>
        /// <returns>Dictionary item id or 0 if item was not found</returns>
        private long? FindPrivacyStatusGuidByCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                DataTable privacyStatusDictionary = this.dataSource.FetchPrivacyStatusDictionary();
                foreach (DataRow row in privacyStatusDictionary.Rows)
                {
                    if (!row.IsNull("Description") && code.Equals(row["Description"].ToString(), StringComparison.OrdinalIgnoreCase) && long.TryParse(row["SysID"].ToString(), out long itemId))
                    {
                        return itemId;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The Order Race
        /// </summary>
        /// <param name="order">AM Order</param>
        /// <returns>Race of Order</returns>
        private Race FhirRace(AMOrder order)
        {
            Race race = null;

            if (order.RaceList.Count > 0)
            {

                // Get Races having valid coding
                List<RaceEthnicityMapperItem> validRaces = this.GetItemsHavingValidCoding(order.RaceList, true);

                if (validRaces == null || (validRaces.Count == 0))
                {
                    return null;
                }

                validRaces = validRaces.OrderBy(i => i.Description).ToList();
                race = new Race();
                race.Categories = new List<Coding>();
                race.Details = new List<Coding>();

                //// Maintain dictionary of the category
                Dictionary<long, string> dictionaryCategorySysIds = new Dictionary<long, string>();
                foreach (var raceItem in validRaces)
                {
                    //// FHIR Race category supports only 5 items
                    if (race.Categories.Count < 5 && raceItem.ParentGuid == 0 && raceItem.Code != null)
                    {
                        dictionaryCategorySysIds.Add(raceItem.SysID, raceItem.Code.Display);

                        race.Categories.Add(raceItem.Code);
                    }
                }

                if (race.Categories.Count > 0)
                {
                    var stringBuilder = new System.Text.StringBuilder();
                    foreach (var item in dictionaryCategorySysIds)
                    {
                        foreach (var raceItem in validRaces)
                        {
                            if (raceItem.IsLowestSelected && raceItem.RootSysID == item.Key && raceItem.Code != null)
                            {
                                race.Details.Add(raceItem.Code);
                                stringBuilder.Append(item.Value);
                                stringBuilder.Append(":");
                                stringBuilder.Append(raceItem.Code.Display);
                                stringBuilder.Append(',');
                            }
                        }

                        if (!stringBuilder.ToString().Contains(item.Value))
                        {
                            stringBuilder.Append(item.Value);
                            stringBuilder.Append(",");
                        }
                    }

                    race.Text = stringBuilder.ToString().TrimEnd(',');
                    if (race.Details.Count == 0)
                    {
                        race.Details = null;
                    }
                }
                else
                {
                    race = null;
                }
            }

            return race;
        }

        /// <summary>
        /// The Order Ethnicity
        /// </summary>
        /// <param name="order">AM Order</param>
        /// <returns>Ethnicity of Order</returns>
        public Ethnicity FhirEthnicity(AMOrder order)
        {
            Ethnicity ethnicity = null;

            if (order.EthnicityList.Count > 0)
            {

                // Get Ethnicity having valid coding
                List<RaceEthnicityMapperItem> validEthnicity = this.GetItemsHavingValidCoding(order.EthnicityList, false);

                if (validEthnicity == null || (validEthnicity.Count == 0))
                {
                    return null;
                }

                validEthnicity = validEthnicity.OrderBy(i => i.Description).ToList();
                ethnicity = new Ethnicity();
                ethnicity.Category = null;
                ethnicity.Details = new List<Coding>();

                //// Maintain dictionary of the category
                Dictionary<long, string> dictionaryCategorySysIds = new Dictionary<long, string>();
                foreach (var ethnicityItem in validEthnicity)
                {
                    //// FHIR ethnicity category supports only 1 ethnicity
                    if (ethnicityItem.ParentGuid == 0 && ethnicityItem.Code != null)
                    {
                        dictionaryCategorySysIds.Add(ethnicityItem.SysID, ethnicityItem.Code.Display);
                        ethnicity.Category = ethnicityItem.Code;
                        break;
                    }
                }

                if (ethnicity.Category != null)
                {
                    var stringBuilder = new System.Text.StringBuilder();
                    foreach (var item in dictionaryCategorySysIds)
                    {
                        foreach (var ethnicityItem in validEthnicity)
                        {
                            if (ethnicityItem.IsLowestSelected && ethnicityItem.RootSysID == item.Key && ethnicityItem.Code != null)
                            {
                                ethnicity.Details.Add(ethnicityItem.Code);
                                stringBuilder.Append(item.Value);
                                stringBuilder.Append(":");
                                stringBuilder.Append(ethnicityItem.Code.Display);
                                stringBuilder.Append(',');
                            }
                        }

                        if (!stringBuilder.ToString().Contains(item.Value))
                        {
                            stringBuilder.Append(item.Value);
                            stringBuilder.Append(",");
                        }
                    }

                    ethnicity.Text = stringBuilder.ToString().TrimEnd(',');
                    if (ethnicity.Details.Count == 0)
                    {
                        ethnicity.Details = null;
                    }
                }
                else
                {
                    ethnicity = null;
                }
            }

            return ethnicity;
        }

        /// <summary>
        /// Get Fhir Extensions
        /// </summary>
        /// <param name="source">AM order</param>
        /// <returns>Fhir Extensions</returns>
        protected virtual List<IExtension> FhirExtensions(AMOrder source)
        {            
            var fhirExtensionList = new List<IExtension>();

            var orderBirthPlaceExtension = GetOrderBirthPlaceExtension(source);
            if (orderBirthPlaceExtension != null)
            {
                fhirExtensionList.Add(orderBirthPlaceExtension);
            }

            var orderInterpreterRequiredExtension = GetOrderInterpreterRequiredExtension(source);
            if (orderInterpreterRequiredExtension != null)
            {
                fhirExtensionList.Add(orderInterpreterRequiredExtension);
            }
           
            var orderReligion = GetOrderReligionExtension(source);
            if (orderReligion != null)
            {
                fhirExtensionList.Add(orderReligion);
            }

            var orderPronoun = GetOrderPronounExtension(source);
            if (orderPronoun != null)
            {
                fhirExtensionList.Add(orderPronoun);
            }

            GenerateProfileExtensions(source, fhirExtensionList);

            return fhirExtensionList.Count > 0 ? fhirExtensionList : null;
        }

        /// <summary>
        /// Generate Profile specific Extensions.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fhirExtensionList"></param>
        public virtual void GenerateProfileExtensions(AMOrder source, List<IExtension> fhirExtensionList)
        {
        }

        /// <summary>
        /// Adds Order Birth Time Extension to Birth Date element
        /// </summary>
        /// <param name="orderDateOfBirth">Order birth date</param>
        /// <param name="dob">FHIR birth date object</param>
        private static void AddOrderBirthTimeExtension(PartialDate orderDateOfBirth, FhirDate dob)
        {
            if (orderDateOfBirth == null || orderDateOfBirth.Year <= 0 || dob == null)
            {
                return;
            }

            var dateOfBirth = new FhirDateTimeOffset();

            if (orderDateOfBirth.DateTimeZoneOffset.HasValue)
            {
                // Date of birth has all five components populated
                dateOfBirth.DateTimeOffset = orderDateOfBirth.DateTimeZoneOffset.Value;
            }
            else
            {
                // Date of birth is represented partially
                dateOfBirth.PartialDate = FormatPartialDate(orderDateOfBirth);
            }

            dob.ElementExtensions ??= new List<IExtension>();

            dob.ElementExtensions.Add(
                new ExampleApp.Fhir.Common.Mdrx.V1.Extensions.OrderBirthTime
                {
                    ExtensionValue = dateOfBirth
                }
            );
        }

        /// <summary>
        /// Returns string representation of partial date
        /// </summary>
        /// <param name="date">Partil date object</param>
        /// <returns>
        /// Returns string representation of partial date
        /// Using format yyyy[-mm[-dd]]
        /// </returns>
        private static String FormatPartialDate(PartialDate date)
        {
            if (date == null)
            {
                return String.Empty;
            }

            var buffer = new StringBuilder();
            if (date.Year > 0)
            {
                buffer.Append($"{date.Year}");
                if (date.Month > 0)
                {
                    buffer.Append($"-{date.Month:00}");
                    if (date.Day > 0)
                    {
                        buffer.Append($"-{date.Day:00}");
                    }
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Adds birth place extension 
        /// </summary>
        /// <param name="source">
        /// Domain order object
        /// </param>
        public IExtension GetOrderBirthPlaceExtension(AMOrder source)
        {
            if (string.IsNullOrEmpty(source.BirthCity) && string.IsNullOrEmpty(source.BirthProvince) &&
                string.IsNullOrEmpty(source.BirthCountry))
            {
                return null;
            }

            var birthAddress = new Address()
                {
                    City = source.BirthCity,
                    State = source.BirthProvince,
                    Country = source.BirthCountry
                };

            return new OrderBirthPlace { ExtensionValue = birthAddress };
        }

        /// <summary>
        /// Adds Interpreter Required extension 
        /// </summary>
        /// <param name="order">
        /// Domain order object
        /// </param>
        public IExtension GetOrderInterpreterRequiredExtension(AMOrder order)
        {
            FhirBool interpreterRequired = new FhirBool(false)
            {
                Value = order.IsInterpreterRequired
            };

            return new OrderInterpreterRequired { ExtensionValue = interpreterRequired };
        }

        /// <summary>
        /// Adds religion extension 
        /// </summary>
        /// <param name="order">
        /// Domain order object
        /// </param>
        public IExtension GetOrderReligionExtension(AMOrder order)
        {
            var religionCodeableConcept = this.GetCodeableConcept("ReligionCode", order.Religion);
            if (religionCodeableConcept == null)
            {
                return null;
            }

            return new OrderReligion { ExtensionValue = religionCodeableConcept };
        }

        /// <summary>
        /// Adds Pronoun extension 
        /// </summary>
        /// <param name="order">
        /// Domain order object
        /// </param>
        protected IExtension GetOrderPronounExtension(AMOrder order)
        {
            FreeFormExtension pronounCodeExtension = new FreeFormExtension();
            var pronounCodeableConcept = this.GetCodeableConcept("PronounsCode", order.PronounCode);
            if (pronounCodeableConcept == null)
            {
                return null;
            }

            pronounCodeExtension.ExtensionValue = pronounCodeableConcept;
            pronounCodeExtension.Name = "individual-pronouns";
            pronounCodeExtension.Url = "http://hl7.org/fhir/StructureDefinition/individual-pronouns";
            return pronounCodeExtension;
        }

        /// <summary>
        /// The Order's Tribal Affiliations
        /// </summary>
        /// <param name="order">AM Order</param>
        /// <returns>Tribal Affiliations of the Order</returns>
        protected virtual List<TribalAffiliation> FhirTribalAffiliation(AMOrder order)
        {
            return null;
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
