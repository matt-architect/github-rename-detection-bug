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
using ExampleApp.Fhir.Common.Mdrx.V1.Infrastructure;

namespace Demo.Fhir.Order.Application.Interfaces.Mapping.Shared
{
    public interface IDomainMapper
    {
        TDest Map<TSource, TDest>(TSource source);

        public TDest Map<TSource, TDest>(TSource source, bool isRedacted = false);

        //public TDest Map<TSource, TDest>(TSource source, bool isSummaryOnly = false, List<FreeFormExtension> extensionNames = null);
    }
}

/* L i c e n s e  N o t i c e */
/*
Confidential and license information of Example Corp. Authorized users only.
Notice to U.S. Government Users: This software is "Commercial Computer Software." Subject to full notice set
forth herein.
*/
/* L i c e n s e  N o t i c e */
