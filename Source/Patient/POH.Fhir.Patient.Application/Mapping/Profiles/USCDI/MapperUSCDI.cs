/* L i c e n s e  N o t i c e */

/* Copyright © 2025 Example Corp. All Rights Reserved.
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
using System.Text;
using System.Threading.Tasks;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Profiles.USCDI;
using Demo.Fhir.Order.Application.Mapping.Profiles.USCDI.V1;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure;

namespace Demo.Fhir.Order.Application.Mapping.Profiles.USCDI
{
    using AMOrder = Domain.Entities.Order;
    using AMContact = Domain.Entities.Contact;
    using AMProvenance = Domain.Entities.Provenance;
    using FhirOrder = ExampleApp.Fhir.Common.Mdrx.V1.Resources.Order;
    using FhirProvenance = ExampleApp.Fhir.Common.Mdrx.V1.Resources.Provenance;
    using FhirOrderContact = ExampleApp.Fhir.Common.Mdrx.V1.BackboneElements.OrderContact;

    public class MapperUSCDI : IMapperUSCDI
    {
        private readonly MapOrderUSCDI _orderMapper;
        private readonly MapContactUSCDI _contactMapper;
        private readonly MapOrderProvenanceUSCDI _provenanceMapper;
        private readonly MapRedactedOrderUSCDI _redactedOrderMapper;

        public MapperUSCDI()
        {
        }

        public MapperUSCDI(
            MapContactUSCDI mapContact,
            MapOrderUSCDI mapOrder,
            MapOrderProvenanceUSCDI mapOrderProvenance,
            MapRedactedOrderUSCDI mapRedactedOrder)
        {
            _contactMapper = mapContact;
            _orderMapper = mapOrder;
            _provenanceMapper = mapOrderProvenance;
            _redactedOrderMapper = mapRedactedOrder;
        }

        public TDest Map<TSource, TDest>(TSource source)
        {
            return Map<TSource, TDest>(source, isRedacted: false);
        }

        public TDest Map<TSource, TDest>(TSource source, bool isRedacted)
        {
            if (source == null)
            {
                return default(TDest);
            }

            // Handle order mapping
            if (source is AMOrder order && typeof(TDest) == typeof(FhirOrder))
            {
                var result = isRedacted 
                    ? _redactedOrderMapper.Map(order)
                    : _orderMapper.Map(order);
                return (TDest)(object)result;
            }

            // Handle contact mapping
            if (source is AMContact contact && typeof(TDest) == typeof(FhirOrderContact))
            {
                var result = _contactMapper.Map(contact);
                return (TDest)(object)result;
            }

            // Handle provenance mapping
            if (source is AMProvenance provenance && typeof(TDest) == typeof(FhirProvenance))
            {
                var result = _provenanceMapper.Map(provenance);
                return (TDest)(object)result;
            }

            throw new NotSupportedException($"Mapping from {typeof(TSource).Name} to {typeof(TDest).Name} is not supported by MapperUSCDI.");
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
