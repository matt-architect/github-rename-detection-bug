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
using POH.BusinessServices.Common.Abstractions;
using POH.BusinessServices.Common.Fhir.Abstractions.CodedConcept;
using POH.BusinessServices.Common.Fhir.Abstractions.CodeSystemLookup;
using POH.BusinessServices.Common.Fhir.Abstractions.SecurityLabels;
using POH.BusinessServices.Common.Fhir.Abstractions.Translation;
using Demo.Fhir.Order.Application.Common.Constants;
using Demo.Fhir.Order.Application.Interfaces;
using Demo.Fhir.Order.Domain.Entities;
using ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure.Interfaces;
using ExampleApp.Fhir.Common.Mdrx.V1.ReferenceTags;
using Address = ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.Address;

namespace Demo.Fhir.Order.Application.Mapping.Shared
{
    using AMContact = Domain.Entities.Contact;
    using AMOrder = Domain.Entities.Order;

    /// <summary>
    /// Class for mapping the AM order to Fast Healthcare Interoperability Resource Order.
    /// </summary>
    public abstract class MapBase : AutoMapper.Profile
    {
        /// <summary>
        /// System URI Address Use.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string SystemURIAddressUse = "AddressUse";

        /// <summary>
        /// System URI Contact point system.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string SystemURIContactPointSystem = "ContactPointSystem";

        /// <summary>
        /// System URI Contact point system.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string SystemURIContactPointUse = "ContactPointUse";

        /// <summary>
        /// Interface translation attribute for Phone Type
        /// </summary>
        protected const string IttAttributePhoneType = "PhoneType";

        /// <summary>
        /// System URI Race
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string SystemURIDetailedRace = "DetailedRace";

        /// <summary>
        /// System URI OMBRaceCategories
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string SystemURIOMBRaceCategories = "OMBRaceCategories";

        /// <summary>
        /// System URI Ethnicity detail
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string SystemURIDetailedEthnicity = "DetailedEthnicity";

        /// <summary>
        /// System URI OMBRaceCategories
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string SystemURIOMBEthnicityCategories = "OMBEthnicityCategories";

        /// <summary>
        /// Prefix for organization representing enterprise
        /// </summary>
        // ReSharper disable once IdentifierTypo
        protected const string ManagingOrgantizationPrefix = "MngOrg-";

        /// <summary>
        /// AM Dictionary GUID column name
        /// </summary>   
        // ReSharper disable once InconsistentNaming
        private const string GUID = "SysID";

        /// <summary>
        /// AM Dictionary Parent GUID column name
        /// </summary>   
        // ReSharper disable once InconsistentNaming
        private const string ParentGUID = "ParentGUID";

        /// <summary>
        /// AM Dictionary Parent ID column name for ethnicity
        /// </summary>   
        // ReSharper disable once InconsistentNaming
        private const string ParentHispanicID = "ParentHispanicID";

        /// <summary>
        /// AM Dictionary Code/Description column name
        /// </summary>   
        private const string Description = "Description";

        /// <summary>
        /// ITT attribute for RaceCodeDetail
        /// </summary>   
        private const string IttAttributeRaceCodeDetail = "RaceCodeDetail";

        /// <summary>
        /// ITT attribute for EthnicDetail
        /// </summary>   
        private const string IttAttributeEthnicDetail = "EthnicDetail";

        /// <summary>
        /// Criteria name for confidential restricted visit
        /// </summary>
        private const string ConfidentialRestrictedVisitCriteria = "Confidential Restricted Encounter";

        /// <summary>
        /// Criteria name for confidential private visit
        /// </summary>
        private const string ConfidentialPrivateVisitCriteria = "Confidential Private Encounter";

        /// <summary>
        /// Criteria name for visit privacy status
        /// </summary>
        private const string VIPPrivacyVisitCriteria = "Visit Privacy Status";

        /// <summary>
        /// Format for Time Period Dates
        /// </summary>
        private const string formatDate = "yyyy-MM-dd";

        /// <summary>
        /// IInterfaceTranslation implementation
        /// </summary>
        private readonly IInterfaceTranslation interfaceTranslation;

        /// <summary>
        /// ICodeSystemLookup implementation
        /// </summary>
        private readonly ICodeSystemLookup codeSysLookup;

        /// <summary>
        /// ISecurityLabelHelper implementation
        /// </summary>
        private readonly ISecurityLabelHelper securityLabelHelper;

        /// <summary>
        /// IWebApiHelper implementation
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        // ReSharper disable once InconsistentNaming
        private readonly IWebApiHelper webAPIHelper;

        /// <summary>
        /// IDataSource implementation
        /// </summary>
        private readonly IDataSource dataSource;

        public MapBase()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapBase" /> class.
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
        internal MapBase(IInterfaceTranslation interfaceTranslation, ICodeSystemLookup codeSysLookup, ISecurityLabelHelper securityLabelHelper, IWebApiHelper webAPIHelper, IDataSource dataSource)
        {
            this.interfaceTranslation = interfaceTranslation;
            this.codeSysLookup = codeSysLookup;
            this.securityLabelHelper = securityLabelHelper;
            this.webAPIHelper = webAPIHelper;
            this.dataSource = dataSource;
        }

        /// <summary>
        /// Gets InterfaceTranslation object.
        /// Provides ITT mapping services to this mapper.
        /// Note: if this isn't thread-safe, neither is the mapper.
        /// </summary>
        protected IInterfaceTranslation InterfaceTranslation => this.interfaceTranslation;

        /// <summary>
        /// Gets CodeSystemLookup object.
        /// <para>
        ///   Provides system mapping services to the mapper.
        /// </para>
        /// <para>
        ///   Can look up local system short hands used in ITTs
        ///   and map them to code system URIs used by FHIR.
        ///   Works in both directions.
        /// </para>
        /// </summary>
        /// <example>
        ///   SNOMEDCT ⟺ <see href="http://snomed.info/sct"/>
        /// </example>
        protected ICodeSystemLookup CodeSysLookup => this.codeSysLookup;

        /// <summary>
        /// Gets Security Label Helper object.
        /// </summary>
        protected ISecurityLabelHelper SecurityLabelHelper => this.securityLabelHelper;

        /// <summary>
        /// Gets Web API helper object.
        /// </summary>
        protected IWebApiHelper WebAPIHelper => this.webAPIHelper;

        /// <summary>
        /// Gets or sets AccessManagerWrapper instance
        /// </summary>
        protected IDataSource AccessManagerWrapper => this.dataSource;

        /// <summary>
        /// Gets a value indicating whether it's for UK.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected bool IsUK
        {
            get
            {
                string connectCountry = this.WebAPIHelper.GetEnvironmentProfile(
                    "Country",
                    "Connect",
                    string.Empty);
                return string.Compare(connectCountry, "GBR", StringComparison.OrdinalIgnoreCase) == 0;
            }
        }

        /// <summary>
        /// Get Coding for passed inputs values.
        /// </summary>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="value">Attribute value</param>
        /// <param name="systemURIName">System URI Name</param>
        /// <param name="translated">Interface translation mapped</param>
        /// <param name="ifIttMissingDoDBLookupToGetSystem">Flag to indicate to do DB Lookup to get System when translation is missing</param>
        /// <returns>Return coding for passed input values.</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected Coding GetCoding(string attributeName, string value, string systemURIName, IInterfaceTranslationMapped translated = null, bool ifIttMissingDoDBLookupToGetSystem = true)
        {
            Coding useCoding = null;

            bool hasItt = true;

            if (translated == null)
            {
                hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                    attributeName,
                    value,
                    out translated);
            }

            CodedConcept codedConcept = null;

            if (hasItt)
            {
                var codeableConcept = this.CodeSysLookup.LookupConcept(translated, value, value, attributeName);

                if (codeableConcept != null)
                {
                    codedConcept = new CodedConcept { Text = codeableConcept.Text };
                    var coding = codeableConcept.Coding?.FirstOrDefault();
                    if (coding != null)
                    {
                        codedConcept.Coding = new List<Code>
                        {
                            new Code(coding.Code, coding.Display, coding.System)
                        };
                    }
                }
            }
            else if (ifIttMissingDoDBLookupToGetSystem)
            {
                this.CodeSysLookup.TryLookupUri(systemURIName, out var systemUri);
                codedConcept = new CodedConcept(value, string.Empty, systemUri);
            }

            if (codedConcept?.Coding != null)
            {
                useCoding = this.ConvertToFhirCoding(codedConcept.Coding).FirstOrDefault();
            }

            return useCoding;
        }

        /// <summary>
        /// Get Coded Concept
        /// </summary>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="value">Attribute value</param>
        /// <param name="systemURIName">System URI Name</param>
        /// <param name="translated">Interface translation mapped</param>
        /// <returns>Coded Concept</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected CodedConcept GetCodedConcept(string attributeName, string value, string systemURIName, IInterfaceTranslationMapped translated = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            bool hasItt = true;

            if (translated == null)
            {
                hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                    attributeName,
                    value,
                    out translated);
            }

            if (!hasItt)
            {
                // if <display> is not set, use untranslated value.
                // If<code> or<system> is not valued, do not populate fields in .coding.Set untranslated value in .text
                return new CodedConcept(value);
            }

            // if itt found.
            var codeableConcept = this.CodeSysLookup.LookupConcept(translated, value, value, attributeName);

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
        /// Convert common code to Fhir Coding
        /// </summary>
        /// <param name="codeList">
        /// List of codes to convert
        /// </param>
        /// <returns>Return Fhir coding List</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        protected List<Coding> ConvertToFhirCoding(List<Code> codeList)
        {
            if (codeList == null || codeList.Count < 1)
            {
                return null;
            }

            var codingList = new List<Coding>();
            codingList.AddRange(codeList.Select(code => new Coding { Code = code.Value, Display = code.Name, System = code.System }));

            return codingList;
        }

        /// <summary>
        /// Get Code able Concept
        /// </summary>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="value">Attribute value</param>
        /// <param name="translated">Interface translation mapped</param>
        /// <returns>Code able Concept</returns>
        protected CodeableConcept GetCodeableConcept(string attributeName, string value, IInterfaceTranslationMapped translated = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            bool hasItt = true;

            if (translated == null)
            {
                hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                    attributeName,
                    value,
                    out translated);
            }

            var codedConcept = this.GetCodedConcept(attributeName, value, string.Empty, translated);

            if (codedConcept == null)
            {
                return null;
            }

            List<Code> codingList = null;
            if (hasItt)
            {
                codingList = codedConcept.Coding;
            }

            var codeableConcept = new CodeableConcept
            {
                Coding = this.ConvertToFhirCoding(codingList),
                Text = codedConcept.Text
            };

            return codeableConcept;
        }

        /// <summary>
        /// Get address from Enterprise address
        /// </summary>
        /// <param name="enterpriseAddress">Enterprise address</param>
        /// <param name = "isAddressUseIttRequired" > Is AddressUse ITT Required</param>
        /// <param name="outputAddressUse"> Output AddressUse</param>
        /// <param name="includeSecurityLabels">Whether security labels need to be included</param>
        /// <returns>Return address</returns>
        protected virtual Address CreateFhirAddressFromEnterpriseAddress(
        Domain.Entities.Address enterpriseAddress,
        bool isAddressUseIttRequired = true,
        bool outputAddressUse = true,
        bool includeSecurityLabels = true)
        {
            if (enterpriseAddress == null || !enterpriseAddress.HasData ||
               (isAddressUseIttRequired && string.IsNullOrEmpty(enterpriseAddress.AddressType)))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(enterpriseAddress.City) &&
                string.IsNullOrWhiteSpace(enterpriseAddress.Country) &&
                string.IsNullOrWhiteSpace(enterpriseAddress.ResidenceCode) &&
                string.IsNullOrWhiteSpace(enterpriseAddress.AddressLine1) &&
                string.IsNullOrWhiteSpace(enterpriseAddress.AddressLine2) &&
                string.IsNullOrWhiteSpace(enterpriseAddress.Zip) &&
                string.IsNullOrWhiteSpace(enterpriseAddress.State))
            {
                return null;
            }

            IInterfaceTranslationMapped translated = null;

            if (isAddressUseIttRequired)
            {
                var hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                    "AddressTypeCode",
                    enterpriseAddress.AddressType,
                    out translated);
                if (!hasItt)
                {
                    return null;
                }
            }

            var addressLines = new List<string>();
            if (!string.IsNullOrWhiteSpace(enterpriseAddress.AddressLine1))
            {
                addressLines.Add(enterpriseAddress.AddressLine1);
            }
            if (!string.IsNullOrWhiteSpace(enterpriseAddress.AddressLine2))
            {
                addressLines.Add(enterpriseAddress.AddressLine2);
            }

            if (enterpriseAddress.IsRedacted)
            {
                enterpriseAddress.IsRedactionWarningRequired = true;
                return CreateRedactedElement<Address>();
            }

            return new Address()
            {
                City = enterpriseAddress.City,
                Country = enterpriseAddress.Country,
                District = enterpriseAddress.ResidenceCode,
                Lines = addressLines,
                Period = GetPeriod(enterpriseAddress.EffectiveDate, enterpriseAddress.ExpiryDate),
                PostalCode = enterpriseAddress.Zip,
                State = enterpriseAddress.State,
                Use = outputAddressUse ? this.GetCoding("AddressTypeCode", enterpriseAddress.AddressType, SystemURIAddressUse, translated) : null,
                Text = this.GetCommaSeparatedValue(new[] { enterpriseAddress.AddressLine1, enterpriseAddress.AddressLine2, enterpriseAddress.City, enterpriseAddress.State, enterpriseAddress.Zip, enterpriseAddress.Country }),
                ElementExtensions = enterpriseAddress.ClientVisitGUID != 0 && includeSecurityLabels ? this.EncounterBasedSecurityTags<Address>(enterpriseAddress.IsRestricted, enterpriseAddress.IsPrivate, enterpriseAddress.VisitPrivacyStatusGUID) : null
            };
        }

        /// <summary>
        /// Get command separated text from list of values
        /// </summary>
        /// <param name="valueList">list of values</param>
        /// <returns>command separated text</returns>
        protected string GetCommaSeparatedValue(string[] valueList)
        {
            var stringBuilder = new System.Text.StringBuilder();
            foreach (var valuePart in valueList)
            {
                if (!string.IsNullOrEmpty(valuePart))
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.Append(valuePart);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Get Human name
        /// </summary>
        /// <param name="enterpriseName">Enterprise name</param>
        /// <param name="use">Coding use</param>
        /// <returns>Human name</returns>
        protected HumanName CreateHumanNameFromEnterpriseName(Name enterpriseName, Coding use)
        {
            if (enterpriseName == null)
            {
                return null;
            }

            var lastName = this.IsUK ? enterpriseName.LastName.ToUpper() : enterpriseName.LastName;

            return new HumanName()
            {
                Family = lastName,
                Given = this.GetListByExcludingEmptyOrNullString(enterpriseName.FirstName, enterpriseName.MiddleName),
                Prefixes = this.GetListByExcludingEmptyOrNullString(enterpriseName.Title),
                Suffixes = this.GetListByExcludingEmptyOrNullString(enterpriseName.Suffix),
                Text = $"{lastName}, {enterpriseName.FirstName} {enterpriseName.MiddleName}".Trim().TrimEnd(','),
                Use = use
            };
        }

        /// <summary>
        /// Get List By Excluding Empty Or Null String
        /// </summary>
        /// <param name="stringValues">String values</param>
        /// <returns>List By Excluding Empty Or Null String</returns>
        protected List<string> GetListByExcludingEmptyOrNullString(params string[] stringValues)
        {
            var strList = stringValues?.Select(str => str).Where(str => !string.IsNullOrEmpty(str)).ToList();

            return strList?.Count > 0 ? strList : null;
        }

        /// <summary>
        /// Get contact point from order phone
        /// </summary>
        /// <param name="enterprisePhone">AM order</param>
        /// <param name="rank">Phone rank</param>
        /// <param name="phoneUseITTattributeName">ITT attributeName for Phone Use</param>
        /// <param name="phoneSytemITTattributeName">ITT attributeName for Phone System</param>
        /// <param name="phoneUseSystemITTFromValue"> ITT from value for Phone Use/System</param>
        /// <param name="includeSecurityLabels">Whether security labels need to be included</param>
        /// <returns>Contact point</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected virtual ContactPoint CreateContactPointFromEnterprisePhone(Phone enterprisePhone, uint? rank, string phoneUseITTattributeName = IttAttributePhoneType, string phoneSytemITTattributeName = "PhoneSystem", string phoneUseSystemITTFromValue = "", bool includeSecurityLabels = true)
        {
            if (enterprisePhone == null || !enterprisePhone.HasData || string.IsNullOrWhiteSpace(enterprisePhone.PhoneNumber))
            {
                return null;
            }

            if (string.IsNullOrEmpty(phoneUseSystemITTFromValue))
            {
                phoneUseSystemITTFromValue = enterprisePhone.PhoneType;
            }

            IInterfaceTranslationMapped translated;
            var hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                phoneUseITTattributeName,
                phoneUseSystemITTFromValue,
                out translated);

            if (!hasItt)
            {
                return null;
            }

            // return redacted contact point if needed
            if (enterprisePhone.IsRedacted)
            {
                // set warning is required flag because phone is redacted in output
                enterprisePhone.IsRedactionWarningRequired = true;
                return CreateRedactedElement<ContactPoint>();
            }

            return new ContactPoint()
            {
                Value = $"{(!string.IsNullOrEmpty(enterprisePhone.CountryCode) ? "+" + enterprisePhone.CountryCode + " " : string.Empty)}({enterprisePhone.AreaCode}) {enterprisePhone.PhoneNumber}".Trim() + (string.IsNullOrEmpty(enterprisePhone.PhoneExtension) ? string.Empty : "-" + enterprisePhone.PhoneExtension).Trim(),
                Period = null,
                Rank = rank,
                System = this.GetCoding(phoneSytemITTattributeName, phoneUseSystemITTFromValue, SystemURIContactPointSystem),
                Use = this.GetCoding(phoneUseITTattributeName, phoneUseSystemITTFromValue, SystemURIContactPointUse, translated),
                ElementExtensions = enterprisePhone.ClientVisitGUID != 0 && includeSecurityLabels ? this.EncounterBasedSecurityTags<ContactPoint>(enterprisePhone.IsRestricted, enterprisePhone.IsPrivate, enterprisePhone.VisitPrivacyStatusGUID) : null
            };
        }

        /// <summary>
        /// Get contact point from order email
        /// </summary>
        /// <param name="order">AM order</param>
        /// <param name="rank">Phone rank</param>
        /// <param name="phoneUseSystemITTFromValue"> ITT from value for Phone Use/System</param>
        /// <param name="phoneUseITTattributeName">ITT attributeName for Phone Use</param>
        /// <param name="phoneSytemITTattributeName">ITT attributeName for Phone System</param>
        /// <returns>Contact point</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected virtual ContactPoint CreateContactPointFromEmailAddress(AMOrder order, uint? rank, string phoneUseSystemITTFromValue, string phoneUseITTattributeName = IttAttributePhoneType, string phoneSytemITTattributeName = "PhoneSystem")
        {
            var orderEmail = order.AddressList.Where(address => address.AddressType == "Email").Select(address => address).FirstOrDefault();

            if (string.IsNullOrEmpty(orderEmail.AddressLine1))
            {
                return null;
            }

            IInterfaceTranslationMapped translated;
            var hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                phoneUseITTattributeName,
                phoneUseSystemITTFromValue,
                out translated);

            if (!hasItt)
            {
                return null;
            }

            return new ContactPoint()
            {
                Value = $"{orderEmail.AddressLine1}",
                Period = null,
                Rank = rank,
                System = this.GetCoding(phoneSytemITTattributeName, phoneUseSystemITTFromValue, SystemURIContactPointSystem),
                Use = this.GetCoding(phoneUseITTattributeName, phoneUseSystemITTFromValue, SystemURIContactPointUse, translated),
            };
        }

        /// <summary>
        /// Get contact point from order email
        /// </summary>
        /// <param name="contact">AM contact</param>
        /// <param name="rank">Phone rank</param>
        /// <param name="phoneUseSystemITTFromValue"> ITT from value for Phone Use/System</param>
        /// <param name="phoneUseITTattributeName">ITT attributeName for Phone Use</param>
        /// <param name="phoneSytemITTattributeName">ITT attributeName for Phone System</param>
        /// <returns>Contact point</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected virtual ContactPoint CreateContactPointFromEmailAddress(AMContact contact, uint? rank, string phoneUseSystemITTFromValue, string phoneUseITTattributeName = IttAttributePhoneType, string phoneSytemITTattributeName = "PhoneSystem")
        {
            if (contact.Email == null)
            {
                return null;
            }

            IInterfaceTranslationMapped translated;
            var hasItt = this.InterfaceTranslation.TryGetMappedForExport(
                phoneUseITTattributeName,
                phoneUseSystemITTFromValue,
                out translated);

            if (!hasItt)
            {
                return null;
            }

            return new ContactPoint()
            {
                Value = $"{contact.Email.AddressLine1}",
                Period = null,
                Rank = rank,
                System = this.GetCoding(phoneSytemITTattributeName, phoneUseSystemITTFromValue, SystemURIContactPointSystem),
                Use = this.GetCoding(phoneUseITTattributeName, phoneUseSystemITTFromValue, SystemURIContactPointUse, translated),
            };
        }

        /// <summary>
        /// Get Races/Ethnicities of the Order having valid code
        /// </summary>
        /// <param name="orderSelectedList">Order selected Races/Ethnicity List is sorted alphabetically and contains all items in the hierarchy</param>
        /// <param name="typeIsRace">Indicate if list is of type Race or Ethnicity</param>
        /// <returns>List of Races/Ethnicities topmost and lowest level selections </returns>
        protected List<RaceEthnicityMapperItem> GetItemsHavingValidCoding(List<string> orderSelectedList, bool typeIsRace)
        {
            DataTable dictionaryDatatable;

            // ReSharper disable once InconsistentNaming
            string parentGUIDColumnName;

            string interfaceTranslationAttributeName;

            // ReSharper disable once InconsistentNaming
            string systemURICategory;

            // ReSharper disable once InconsistentNaming
            string systemURIDetail;

            if (typeIsRace)
            {
                parentGUIDColumnName = ParentGUID;
                interfaceTranslationAttributeName = IttAttributeRaceCodeDetail;
                systemURICategory = SystemURIOMBRaceCategories;
                systemURIDetail = SystemURIDetailedRace;
                dictionaryDatatable = this.AccessManagerWrapper.FetchRaceDictionary();
            }
            else
            {
                systemURICategory = SystemURIOMBEthnicityCategories;
                systemURIDetail = SystemURIDetailedEthnicity;
                parentGUIDColumnName = ParentHispanicID;
                interfaceTranslationAttributeName = IttAttributeEthnicDetail;
                dictionaryDatatable = this.AccessManagerWrapper.FetchEthnicityDictionary();
            }

            //// check if dataset/table exist
            if (dictionaryDatatable == null || dictionaryDatatable.Rows.Count == 0)
            {
                return null;
            }

            //// Datatable with only selected items from OrderEntity.RaceList
            DataTable selectedEntriesDatatable = dictionaryDatatable.AsEnumerable()
                .Where(row => orderSelectedList.Contains(row["Description"]))
                .CopyToDataTable();

            this.RemoveInactiveDuplicates(selectedEntriesDatatable);

            // Create list of Objects from the datatable.
            var mappedItemList = new List<RaceEthnicityMapperItem>();
            foreach (DataRow item in selectedEntriesDatatable.Rows)
            {
                RaceEthnicityMapperItem raceObject = new RaceEthnicityMapperItem
                {
                    SysID = Convert.ToInt64(item[GUID]),
                    Description = Convert.ToString(item[Description]),
                    ParentGuid = Convert.IsDBNull(item[parentGUIDColumnName]) ? 0 : Convert.ToInt64(item[parentGUIDColumnName])
                };

                // ReSharper disable once InconsistentNaming
                var systemURL = systemURIDetail;

                if (raceObject.ParentGuid == 0)
                {
                    systemURL = systemURICategory;
                }
                //// Set Code
                raceObject.Code = this.GetCoding(interfaceTranslationAttributeName, raceObject.Description, systemURL, null, false);

                mappedItemList.Add(raceObject);
            }

            mappedItemList = mappedItemList.DistinctBy(a => a.Description).ToList();
            List<RaceEthnicityMapperItem> lowestNodes;

            //// Create a new list to track only lowest level. The lowest level race's SysID(GUID) will not be a ParentGUID to any other race in the list
            var list = mappedItemList;

            lowestNodes = mappedItemList.Where(p => list.All(p2 => p2.ParentGuid != p.SysID))
                .DistinctBy(a => a.Description).ToList();

            //// Set the flag for IsLowestSelected only if Lowestnode and the Root node has ITT configured.
            foreach (var item in mappedItemList)
            {
                var lowestItem = lowestNodes.FirstOrDefault(d => d.Description == item.Description);
                if (lowestItem != null && lowestItem.Code != null)
                {
                    var rootParent = this.FindRootParent(item.ParentGuid, ref mappedItemList);
                    if (rootParent != null && rootParent.Code != null)
                    {
                        item.RootSysID = rootParent.SysID;
                        item.IsLowestSelected = true;
                    }
                }
            }

            //// return only items that can be added as a category or detail on the FHIR race object
            mappedItemList = mappedItemList.Where(r => r.Code != null && (r.ParentGuid == 0 || r.IsLowestSelected)).ToList();

            return mappedItemList;
        }

        /// <summary>
        /// Find the root race/ethnicity node by recursively iterating through the hierarchy in the List
        /// </summary>
        /// <param name="guid">The race/ethnicity guid</param>
        /// <param name="raceEthnicityList">List of order selected races</param>
        /// <returns>The top most (root/category) parent</returns>
        protected RaceEthnicityMapperItem FindRootParent(long guid, ref List<RaceEthnicityMapperItem> raceEthnicityList)
        {
            RaceEthnicityMapperItem itemRoot;
            RaceEthnicityMapperItem item;
            item = raceEthnicityList.Where(r => r.SysID == guid).FirstOrDefault();
            if (item == null)
            {
                return null;
            }

            if (item.ParentGuid == 0)
            {
                itemRoot = item;
            }
            else
            {
                itemRoot = this.FindRootParent(item.ParentGuid, ref raceEthnicityList);
            }

            return itemRoot;
        }

        /// <summary>
        /// Check if any duplicates exists and remove inactive entries.
        /// </summary>
        /// <param name="dataTableSelectedEntries">data table with the selected entries matching AM dictionary.</param>
        protected void RemoveInactiveDuplicates(DataTable dataTableSelectedEntries)
        {
            var hashTable = new Dictionary<string, DataRow>();
            var rowsForDeletion = new List<DataRow>();

            foreach (DataRow row in dataTableSelectedEntries.Rows)
            {
                var key = row["Description"] as string;
                if (!string.IsNullOrEmpty(key))
                {
                    if (!hashTable.ContainsKey(key))
                    {
                        hashTable[key] = row;
                    }
                    else
                    {
                        // duplicated row found
                        var active = row["Active"].ToString();
                        if ("false".Equals(active, StringComparison.OrdinalIgnoreCase))
                        {
                            rowsForDeletion.Add(row);
                        }
                        else
                        {
                            rowsForDeletion.Add(hashTable[key]);
                        }
                    }
                }
            }

            foreach (DataRow row in rowsForDeletion)
            {
                dataTableSelectedEntries.Rows.Remove(row);
            }
        }

        /// <summary>
        /// Loads security labels for elements associated with confidential visit
        /// </summary>
        /// <param name="isRestricted">
        /// Value indicating whether element associated with restricted visit 
        /// </param>
        /// <param name="isPrivate">
        /// Value indicating whether element associated with private visit 
        /// </param>
        /// <param name="visitPrivacyStatusGuid">
        /// Value indicating the visit's privacy status guid
        /// </param>
        /// <returns>
        /// List of extensions containing security labels
        /// </returns>
        protected List<IExtension> EncounterBasedSecurityTags<T>(bool isRestricted, bool isPrivate, long visitPrivacyStatusGuid)
        {
            var tags = new List<IExtension>();

            var securityLabels = new List<Coding>();
            this.SecurityLabelHelper.GetSecurityLabel(securityLabels, ResourceName.AllEncounterBased, ConfidentialRestrictedVisitCriteria, isRestricted.ToString());
            this.SecurityLabelHelper.GetSecurityLabel(securityLabels, ResourceName.AllEncounterBased, ConfidentialPrivateVisitCriteria, isPrivate.ToString());
            this.SecurityLabelHelper.GetSecurityLabel(securityLabels, ResourceName.AllEncounterBased, VIPPrivacyVisitCriteria, visitPrivacyStatusGuid);

            securityLabels.ForEach(securityLabel =>
            {
                tags.Add(new FreeFormExtension
                {
                    Name = GetExtensionNameByElementType<T>(),
                    ExtensionValue = securityLabel
                });
            });

            return tags.Count > 0 ? tags : null;
        }

        /// <summary>
        /// Creates empty FhirElement with redacted label
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">Context for GetExtensionNameByElementType in cases where multiple elements share same T : FhirElement</param>
        /// <returns></returns>
        protected T CreateRedactedElement<T>(string context = null) where T : FhirElement, new()
        {
            var element = new T { ElementExtensions = new List<IExtension>() };
            AddRedactedLabel(element, context);
            return element;
        }

        /// <summary>
        /// Adds element extension with redaction label
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="element">Element instance</param>
        /// <param name="context">Context for GetExtensionNameByElementType in cases where multiple elements share same T : FhirElement</param>
        protected void AddRedactedLabel<T>(T element, string context = null) where T : FhirElement
        {
            if (element.ElementExtensions == null)
            {
                element.ElementExtensions = new List<IExtension>();
            }

            var label = CreateRedactionLabel();
            element.ElementExtensions.Add(new FreeFormExtension { Name = GetExtensionNameByElementType<T>(context), ExtensionValue = label });
        }

        /// <summary>
        /// Creates new redaction security label
        /// </summary>
        /// <returns>Returns Coding representing redaction label</returns>
        private Coding CreateRedactionLabel()
        {
            var labels = this.securityLabelHelper.GetRedactedSecurityLabel();
            if (labels != null && labels.Count > 0)
            {
                return labels[0];
            }

            // should not come here
            // just in case
            // hardcoded value
            // REDACT^redact^http://terminology.hl7.org/CodeSystem/V3-ObservationValue
            return new Coding
            {
                Code = Constants.RedactionLabelText.ToUpper(),
                Display = Constants.RedactionLabelText.ToLower(),
                System = Constants.RedactionLabelSystem
            };
        }

        /// <summary>
        /// Get Period from Effective Date and Expiry Date
        /// </summary>
        /// <param name="effectiveDate">Effective date time</param>
        /// <param name = "expiryDate" >Expiry date time</param>
        /// <returns>Return Period date</returns>
        public static TimePeriod GetPeriod(DateTime? effectiveDate, DateTime? expiryDate)
        {
            if (effectiveDate == null && expiryDate == null)
            {
                return null;
            }

            var period = new TimePeriod();

            if (effectiveDate != null)
            {
                period.Start = new FhirDateTimeOffset() { PartialDate = effectiveDate?.Date.ToString(formatDate) };
            }

            if (expiryDate != null)
            {
                period.End = new FhirDateTimeOffset() { PartialDate = expiryDate?.Date.ToString(formatDate) };
            }

            return period;
        }

        /// <summary>
        /// Returns extension name for specific element type
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="context">Context for cases where multiple elements share same T : FhirElement</param>
        /// <returns>Returns extension name for specific element type</returns>
        /// <exception cref="ApplicationException"></exception>
        private string GetExtensionNameByElementType<T>(string context = null)
        {
            string typeName = typeof(T).FullName;

            switch (typeName)
            {
                case @"ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.Address":
                    return "DS4P-OrderAddress";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.ContactPoint":
                    return @"DS4P-OrderTelecom";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.OrderContact":
                    return "DS4P-OrderContact";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.Resources.Practitioner":
                    return "DS4P-OrderGeneralPractitioner";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.Identifier":
                    return "DS4P-OrderIdentifier";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.Extensions.Sex":
                    return "DS4P-OrderSex";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.Extensions.GenderIdentity":
                    return "DS4P-OrderGenderIdentity";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.Race":
                    return "DS4P-OrderRace";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.Ethnicity":
                    return "DS4P-OrderEthnicity";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.TribalAffiliation":
                    return "DS4P-OrderTribalAffiliation";

                case @"ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.Coding":
                    return context switch
                    {
                        "BirthGender" => "DS4P-OrderBirthSex",
                        "Gender" => "DS4P-OrderGender",
                        _ => "DS4P-OrderBirthSex"
                    };

                default:
                    throw new ApplicationException($"Extension name requested for unsupported element type: {typeName}");
            }
        }

        /// <summary>
        /// Creates an object version extension
        /// </summary>
        /// <param name="name">The extension name</param>
        /// <param name="url">The extension URL</param>
        /// <param name="objectVersion">The object version value</param>
        /// <param name="elementId">The element ID</param>
        /// <returns>A FreeFormExtension containing the object version</returns>
        protected FreeFormExtension CreateObjectVersionExtension(string name, string objectVersion, string elementId)
        {
            String url = $"{Constants.ObjectVersionURLPrefix}{name}";

            return new FreeFormExtension
            {
                Name = name,
                Url = url,
                ExtensionValue = new FhirString(objectVersion),
                ElementID = elementId
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
