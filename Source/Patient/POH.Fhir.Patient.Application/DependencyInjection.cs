/* L i c e n s e  N o t i c e */

/* Copyright © 2024-2025 Example Corp. All Rights Reserved.
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
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Demo.Fhir.Order.Application.Helpers;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Profiles.UK;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Profiles.USCDI;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Profiles.USCDI.V1;
using Demo.Fhir.Order.Application.Interfaces.Mapping.Profiles.USCDI.V3;
using Demo.Fhir.Order.Application.Mapping;
using Demo.Fhir.Order.Application.Mapping.OrderHeader;
using Demo.Fhir.Order.Application.Mapping.Profiles.UK;
using Demo.Fhir.Order.Application.Mapping.Profiles.USCDI;
using Demo.Fhir.Order.Application.Mapping.Profiles.USCDI.V1;
using Demo.Fhir.Order.Application.Mapping.Profiles.USCDI.v3;
using Demo.Fhir.Order.Application.Mapping.Profiles.USCDI.V3;
using Demo.Fhir.Order.Application.Mapping.Shared;
using Demo.Fhir.Order.Application.Profiles.USCDI.V3.Handlers;
using Demo.Fhir.Order.Application.Profiles.USCDI.V3.Queries;
using Demo.Fhir.Order.Domain.Entities;
using POH.Infrastructure.Mediator;

namespace Demo.Fhir.Order.Application;

/// <summary>
/// DependencyInjection
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// AddApplication
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register mediator service manually without automatic handler scanning.
        // Handlers are explicitly registered in AddRequests() with their concrete mapper types.
        services.AddScoped<IMediator, Mediator>();

        services.AddScoped<IServiceWrapper, ServiceWrapper>();
        services.AddSingleton<IValidation, Validation>();

        // Register USCDI mappers and interfaces
        services.AddScoped<MapOrderUSCDI>();
        services.AddScoped<MapOrderProvenanceUSCDI>();
        services.AddScoped<MapContactUSCDI>();
        services.AddScoped<MapRedactedOrderUSCDI>();
        services.AddScoped<IMapperUSCDI, MapperUSCDI>();

        // Register USCDI v1 mappers and interfaces
        services.AddScoped<MapContactUSCDIV1>();
        services.AddScoped<MapOrderProvenanceUSCDIV1>();
        services.AddScoped<MapOrderUSCDIV1>();
        services.AddScoped<MapRedactedOrderUSCDIV1>();
        services.AddScoped<IMapperUSCDIV1, MapperUSCDIV1>();

        // Register USCDI v3 mappers and interfaces
        services.AddScoped<MapContactUSCDIV3>();
        services.AddScoped<MapOrderProvenanceUSCDIV3>();
        services.AddScoped<MapOrderUSCDIV3>();
        services.AddScoped<MapRedactedOrderUSCDIV3>();
        services.AddScoped<IMapperUSCDIV3, MapperUSCDIV3>();

        // Register UK mappers and interfaces
        services.AddScoped<MapContactUK>();
        services.AddScoped<MapOrderProvenanceUK>();
        services.AddScoped<MapOrderUK>();
        services.AddScoped<MapRedactedOrderBase, MapRedactedOrderUK>();
        services.AddScoped<IMapperUK, MapperUK>();

        // Register OrderHeader mapper
        services.AddScoped<IOrderHeaderMapper, OrderHeaderMapper>();

        return services;
    }
}

/* L i c e n s e  N o t i c e */
/*
Confidential and license information of Example Corp. Authorized users only.
Notice to U.S. Government Users: This software is "Commercial Computer Software." Subject to full notice set
forth herein.
*/
/* L i c e n s e  N o t i c e */