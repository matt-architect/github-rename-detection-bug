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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using POH.BusinessServices.Common.Abstractions;
using POH.BusinessServices.Common.Fhir.Abstractions.CodedConcept;
using POH.BusinessServices.Common.Fhir.Abstractions.CodeSystemLookup;
using POH.BusinessServices.Common.Fhir.Abstractions.SecurityLabels;
using POH.BusinessServices.Common.Fhir.Abstractions.Translation;
using POH.BusinessServices.Common.Fhir.Extensions;
using Demo.Fhir.Order.Application.Common.Constants;
using Demo.Fhir.Order.Application.Interfaces;
using Demo.Fhir.Order.Domain.Entities;
using ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure.Interfaces;
using Address = ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.Address;

namespace Demo.Fhir.Order.Application.Mapping.Shared
{
    using AMContact = Contact;

    /// <summary>
    /// Class for mapping the AM order's contact to Fast Healthcare Interoperability Resource Order's contact.
    /// </summary>
    public class MapContactBase : MapBase, IContactMapper
    {
        /// <summary>
        /// Interface translation attribute for Contact Phone Type
        /// </summary>
        public const string IttAttributeContactPhoneType = "ContactPhoneType";

        /// <summary>
        /// Interface translation attribute for Contact Phone System
        /// </summary>
        public const string IttAttributeContactPhoneSystem = "ContactPhoneSystem";

        /// <summary>
        ///   Initializes a new instance of the <see cref="MapContactBase"/> class.
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
        public MapContactBase(IInterfaceTranslation interfaceTranslation, ICodeSystemLookup codeSysLookup, ISecurityLabelHelper securityLabelHelper, IWebApiHelper webAPIHelper, IDataSource dataSource) :
            base(interfaceTranslation, codeSysLookup, securityLabelHelper, webAPIHelper, dataSource)
        {
        }

        /// <summary>
        /// Maps an AM Contact to a FHIR OrderContact.
        /// </summary>
        /// <param name="source">The source AM Contact entity</param>
        /// <returns>A FHIR OrderContact backbone element</returns>
        public virtual OrderContact Map(AMContact source)
        {
            if (source == null)
            {
                return null;
            }

            var contactPoints = FhirContactPoint(source);
            var relationships = FhirRelationship(source);

            FhirList<ContactPoint> fhirContactPoints = null;
            if (contactPoints != null && contactPoints.Count > 0)
            {
                fhirContactPoints = new FhirList<ContactPoint>();
                foreach (var cp in contactPoints)
                {
                    fhirContactPoints.Add(cp);
                }
            }

            List<CodeableConcept> fhirRelationships = null;
            if (relationships != null)
            {
                fhirRelationships = relationships.Select(cc => new CodeableConcept
                {
                    Text = cc.Text,
                    Coding = cc.Coding?.Select(code => new Coding
                    {
                        Code = code.Value,
                        Display = code.Name,
                        System = code.System
                    }).ToList()
                }).ToList();
            }

            var destination = new OrderContact
            {
                Address = FhirAddress(source),
                ContactPoints = fhirContactPoints,
                Name = FhirName(source),
                Relationships = fhirRelationships,
                ElementExtensions = ContactSecurityTags(source)
            };

            AfterMap(source, destination);
            return destination;
        }

        /// <summary>
        /// Get address
        /// </summary>
        /// <param name="contact">AM contact</param>
        /// <returns>Returns contact address</returns>
        protected virtual Address FhirAddress(AMContact contact)
        {
            return this.CreateFhirAddressFromEnterpriseAddress(contact.ContactAddress, false, false, includeSecurityLabels: false);
        }

        /// <summary>
        /// Get Human names
        /// </summary>
        /// <param name="contact">AM contact</param>
        /// <returns>Human name list</returns>
        protected virtual HumanName FhirName(AMContact contact)
        {
            return this.CreateHumanNameFromEnterpriseName(contact.Name, null);
        }

        /// <summary>
        /// Get Relationship
        /// </summary>
        /// <param name="contact">AM contact</param>
        /// <returns>Returns contact Relationship</returns>
        protected virtual List<CodedConcept> FhirRelationship(AMContact contact)
        {
            var contactTypeCodedConcept = this.GetCodedConcept("ContactTypeCode", contact.ContactType, "OrderContactRelationship");
            var relationshipCodedConcept = this.GetCodedConcept("RelationshipCode", contact.RelationshipType, string.Empty);

            if (contactTypeCodedConcept == null && relationshipCodedConcept == null)
            {
                return null;
            }

            var textBuilder = new StringBuilder();
            var concept = new CodedConcept { Coding = new List<Code>() };

            if (contactTypeCodedConcept != null)
            {
                // List.AddRange does not allow null argument
                // Coding can be null if ITT URI or namespace is not correct
                if (contactTypeCodedConcept.Coding != null)
                {
                    concept.Coding.AddRange(contactTypeCodedConcept.Coding);
                }

                textBuilder.Append($"Contact Role: {contactTypeCodedConcept.Text}");
            }

            if (relationshipCodedConcept != null)
            {
                // List.AddRange does not allow null argument
                // Coding can be null if ITT URI or namespace is not correct
                if (relationshipCodedConcept.Coding != null)
                {
                    concept.Coding.AddRange(relationshipCodedConcept.Coding);
                }

                if (textBuilder.Length > 0)
                {
                    textBuilder.Append(" | ");
                }

                textBuilder.Append($"Relationship Role Type: {relationshipCodedConcept.Text}");
            }

            concept.Text = textBuilder.ToString();

            return new List<CodedConcept> { concept };
        }

        /// <summary>
        /// Get Contact point
        /// </summary>
        /// <param name="contact">AM contact</param>
        /// <returns>Contact point</returns>
        protected virtual List<ContactPoint> FhirContactPoint(AMContact contact)
        {
            List<ContactPoint> contactPoint = new List<ContactPoint>();

            var homePhone = this.CreateContactPointFromEnterprisePhone(contact.HomePhone, null, IttAttributeContactPhoneType, IttAttributeContactPhoneSystem, "Home", includeSecurityLabels: false);
            if (homePhone != null)
            {
                contactPoint.Add(homePhone);
            }

            var businessPhone = this.CreateContactPointFromEnterprisePhone(contact.BusinessPhone, null, IttAttributeContactPhoneType, IttAttributeContactPhoneSystem, "Business", includeSecurityLabels: false);
            if (businessPhone != null)
            {
                contactPoint.Add(businessPhone);
            }

            var alternatePhone = this.CreateContactPointFromEnterprisePhone(contact.AlternatePhone, null, IttAttributeContactPhoneType, IttAttributeContactPhoneSystem, "Alternate", includeSecurityLabels: false);
            if (alternatePhone != null)
            {
                contactPoint.Add(alternatePhone);
            }

            var faxPhone = this.CreateContactPointFromEnterprisePhone(contact.Fax, null, IttAttributeContactPhoneType, IttAttributeContactPhoneSystem, "Fax", includeSecurityLabels: false);
            if (faxPhone != null)
            {
                contactPoint.Add(faxPhone);
            }

            var email = this.CreateContactPointFromEmailAddress(contact, null, "Email", IttAttributeContactPhoneType, IttAttributeContactPhoneSystem);
            if (email != null)
            {
                contactPoint.Add(email);
            }

            return contactPoint;
        }

        /// <summary>
        /// Produces security labels for contact
        /// </summary>
        /// <param name="contact">AM contact</param>
        /// <returns>List of extensions containing security labels</returns>
        private List<IExtension> ContactSecurityTags(AMContact contact)
        {
            return contact.ClientVisitGUID != 0 ? this.EncounterBasedSecurityTags<OrderContact>(contact.IsRestricted, contact.IsPrivate, contact.VisitPrivacyStatusGUID) : null;
        }

        /// <summary>
        /// makes final changes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        protected virtual void AfterMap(AMContact source, OrderContact destination)
        {
            if (source.IsRedacted)
            {
                // set warning is required flag because contact is redacted in output
                source.IsRedactionWarningRequired = true;
                RedactElement(destination);
            }
        }

        /// <summary>
        /// Removes all properties of element and adds redaction label
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="element">Element object</param>
        public void RedactElement<T>(T element) where T : FhirElement
        {
            var propertyInfoList = element.GetType().GetProperties().ToList();
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                propertyInfo.SetValue(element, null);
            }

            AddRedactedLabel(element);
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
