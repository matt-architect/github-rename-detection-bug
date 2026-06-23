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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using POH.BusinessServices.Common.Abstractions;
using POH.BusinessServices.Common.Fhir.Abstractions.CodeSystemLookup;
using POH.BusinessServices.Common.Fhir.Abstractions.SecurityLabels;
using POH.BusinessServices.Common.Fhir.Abstractions.Translation;
using POH.BusinessServices.Common.Fhir.Mapping;
using Demo.Fhir.Order.Application.Interfaces;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Profiles.UK;
using Demo.Fhir.Order.Application.Mapping.Shared;
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure;

namespace Demo.Fhir.Order.Application.Mapping.Profiles.UK
{
    public class MapperUK : IMapperUK
    {
        private readonly IMapper _mapper;
        private readonly IMapper _mapperRedacted;
        private readonly MapOrderUK _mapOrder;

        public MapperUK()
        {
        }

        public MapperUK(
            CommonMaps commonMaps,
            MapContactUK mapContact,
            MapOrderUK mapOrder,
            MapOrderProvenanceUK mapOrderProvenance,
            MapRedactedOrderBase mapRedactedOrder)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(commonMaps);
                cfg.AddProfile(mapContact);
                cfg.AddProfile(mapOrder);
                cfg.AddProfile(mapOrderProvenance);
            });

            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();

            var configRedacted = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(mapRedactedOrder);
            });

            configRedacted.AssertConfigurationIsValid();
            _mapperRedacted = configRedacted.CreateMapper();
        }

        public TDest Map<TSource, TDest>(TSource source)
        {
            return _mapper.Map<TDest>(source);
        }

        public TDest Map<TSource, TDest>(TSource source, bool isRedacted = false)
        {
            return isRedacted ? _mapperRedacted.Map<TSource, TDest>(source) : _mapper.Map<TSource, TDest>(source);
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
