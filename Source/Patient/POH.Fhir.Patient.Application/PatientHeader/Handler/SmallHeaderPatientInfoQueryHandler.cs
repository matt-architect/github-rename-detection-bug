/* Copyright © 2026 Example Corp. All Rights Reserved.
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

using AutoMapper;
using FluentResults;
using Demo.Fhir.Order.Application.Interfaces.Repositories.OrderHeader;
using Demo.Fhir.Order.Application.OrderHeader.Models.Response;
using Demo.Fhir.Order.Application.OrderHeader.Queries;
using Demo.Fhir.Order.Domain.Entities;
using POH.Infrastructure.Mediator;
using ExampleApp.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Fhir.Order.Application.OrderHeader.Handler
{
    public class SmallHeaderOrderInfoQueryHandler : IRequestHandler<SmallHeaderOrderInfoQuery, Result<SmallHeaderOrderInfoResponse>>
    {
       
        private const string HandlerName = nameof(SmallHeaderOrderInfoQueryHandler);
        private readonly IMapper _mapper;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IAppLogger<SmallHeaderOrderInfoQueryHandler> _logger;

        public SmallHeaderOrderInfoQueryHandler(
            IOrderHeaderRepository orderHeaderRepository,
            IMapper mapper,
            IAppLogger<SmallHeaderOrderInfoQueryHandler> logger)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<SmallHeaderOrderInfoResponse>> Handle(SmallHeaderOrderInfoQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{Handler} start.", HandlerName);

            // Step 1: Build parallel tasks for BOTH BasicInfo and Photo
            var tasks = new List<Task>();
            Result<OrderHeaderOrderInfoEntity> basicInfoResult = null;
            Result<OrderImage> photoResult = null;

            //Add BasicInfo task(required - but runs in parallel)
            tasks.Add(RunDomainAsync("BasicInfo", async () =>
            {
                basicInfoResult = await _orderHeaderRepository.GetBasicOrderInfo(request.ClientGuid, request.VisitGuid, request.UserGuid);
                return basicInfoResult.IsSuccess;
            }));

            // Add Photo task (optional - runs in parallel)
            tasks.Add(RunDomainAsync("Photo", async () =>
            {
                photoResult = await _orderHeaderRepository.GetOrderPhoto(request.ClientGuid);
                return photoResult.IsSuccess;
            }));


            // Step 2: Execute ALL tasks in parallel
            if (tasks.Count > 0)
            {
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    _logger.LogError("{Handler} unexpected aggregate exception. Exception: {ExceptionMessage}", HandlerName, ex.Message);
                }
            }

            // Step 3: Check if BasicInfo succeeded (required - blocking validation)
            if (basicInfoResult == null || basicInfoResult.IsFailed)
            {
                _logger.LogError("{Handler} BasicInfo retrieval failed. Cannot proceed without order data. Error: {Error}",
                    HandlerName, basicInfoResult?.Errors?.FirstOrDefault()?.Message ?? "BasicInfo returned null");
                return Result.Fail<SmallHeaderOrderInfoResponse>(
                    $"Small Header Order Info Errors - {basicInfoResult?.Errors?.FirstOrDefault()?.Message ?? "BasicInfo failed"}");
            }

            // Step 4: Map BasicInfo to Response
            var response = _mapper.Map<SmallHeaderOrderInfoResponse>(basicInfoResult.Value);

            // Step 5: Add Photo to response if available (non-blocking failure)
            if (photoResult?.IsSuccess == true && photoResult.Value != null)
            {
                response.FullImageEncoded = photoResult.Value.FullImageEncoded;
                _logger.LogDebug("{Handler} Photo added to response.", HandlerName);
            }
            else
            {
                _logger.LogError("{Handler} Photo not available or failed to load. Error: {Error}",
                    HandlerName, photoResult?.Errors?.FirstOrDefault()?.Message ?? "Photo not found");
            }

            _logger.LogDebug("{Handler} completed.", HandlerName);

            return Result.Ok(response);

            // Local helper to execute and log each domain call
            async Task RunDomainAsync(string domainName, Func<Task<bool>> action)
            {
                var sw = Stopwatch.StartNew();
                bool success = false;
                string error = null;

                try
                {
                    _logger.LogDebug("Domain {Domain} start.", domainName);
                    success = await action();
                    _logger.LogInfo("Domain {Domain} completed. Success={Success} ElapsedMs={Elapsed}", domainName, success, sw.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    _logger.LogError("Domain {Domain} failed after {Elapsed}ms. Exception: {ExceptionMessage}", domainName, sw.ElapsedMilliseconds, ex.Message);
                }
                finally
                {
                    sw.Stop();
                }
            }
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