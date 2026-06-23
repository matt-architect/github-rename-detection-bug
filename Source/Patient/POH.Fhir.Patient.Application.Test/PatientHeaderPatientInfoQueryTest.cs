/* Copyright © 2025 - 2026 Example Corp. All Rights Reserved.
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
using Dapper;
using FluentResults;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Demo.Fhir.Order.Application.Interfaces.Repositories.OrderHeader;
using Demo.Fhir.Order.Application.Mapping.OrderHeader;
using Demo.Fhir.Order.Application.OrderHeader.Handler;
using Demo.Fhir.Order.Application.OrderHeader.Queries;
using Demo.Fhir.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Fhir.Order.Application.Test
{
    [TestClass]
    public class OrderHeaderOrderInfoQueryTest
    {
        private Mock<IOrderHeaderRepository> _repository;
        private MapperConfiguration _mapperConfiguration;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IOrderHeaderRepository>();
            _mapperConfiguration = new MapperConfiguration(
                cfg => cfg.AddProfile(new OrderHeaderOrderInfoProfile()));
            _mapper = new Mapper(_mapperConfiguration);
        }




        [TestMethod]
        public async Task TestGetOrderHeaderOrderInfoQuery_Return_Success()
        {
            long ClientGUID = 1442800200;
            long VisitGuid = 1234;
            long UserGuid = 5678;   
            // Setup entity data
            OrderHeaderOrderInfoEntity mockOrderInfoEntity = new OrderHeaderOrderInfoEntity
            {
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "M",
                Suffix = "Jr",
                MRN = "100007",
                DisplayName = "John Doe",
                PreferredName = "John",
                ClientAge = 30,
                Pronouns = "John",
                ClientVisitNumber = "123",
                AgeDisplayValue = "43y",
                InternalVisitStatus = "ADM",
                VisitStatusDisplayName = "Admitted",
                AdmittedDateAndTime = DateTime.Parse("2024-06-11T07:34:00"),
                AdmittedDate = "11 Jun 2024",
                AdmittedTime = "07:34:00",
                DischargedDateAndTime = DateTime.Parse("2024-06-15T15:34:00"),
                DischargedDate = "15 Jun 2024",
                DischargedTime = "15:34:00",
                PlannedDischargedDateAndTime = DateTime.Parse("2024-06-15T00:00:00"),
                PlannedDischargedDate = "15 Jun 2024",
                TimeZoneFriendlyName = "Eastern Standard Time",
                TimeZone = "Eastern Standard Time",
                IsOrderBalanceEnabled = true,
                FormattedPhone = "(123) 456-7890",
                InsuranceCarrier = "Aetna",         
                InsurancePlan = "Aetna Plan",
                PolicyNumber = "43545464656565"

            };

            OrderHeaderOrderInfoEntity response = mockOrderInfoEntity;
            var result = new Result<OrderHeaderOrderInfoEntity>().WithValue(response);

            _repository.Setup(r => r.GetOrderHeaderOrderInfo(ClientGUID, VisitGuid, UserGuid))
                       .ReturnsAsync(result);

            var queryHandler = new OrderHeaderOrderInfoQueryHandler(_repository.Object, _mapper);
            var query = new OrderHeaderOrderInfoQuery()
            {
                ClientGuid = ClientGUID,
                VisitGuid = VisitGuid,
                UserGuid = UserGuid
            };

            var task = await queryHandler.Handle(query, new CancellationToken());

            // Assert
            Assert.IsNotNull(task, "Task should not be null");
            Assert.AreEqual(mockOrderInfoEntity.MRN, task.Value.MRN);
            Assert.AreEqual(mockOrderInfoEntity.InternalVisitStatus, task.Value.InternalVisitStatus);
            Assert.AreEqual(mockOrderInfoEntity.VisitStatusDisplayName, task.Value.VisitStatusDisplayName);
            Assert.AreEqual(mockOrderInfoEntity.AdmittedDateAndTime, task.Value.AdmittedDateAndTime);
            Assert.AreEqual(mockOrderInfoEntity.DischargedDateAndTime, task.Value.DischargedDateAndTime);
            Assert.AreEqual(mockOrderInfoEntity.PlannedDischargedDateAndTime, task.Value.PlannedDischargedDateAndTime);
            Assert.AreEqual(mockOrderInfoEntity.TimeZoneFriendlyName, task.Value.TimeZoneFriendlyName);
            Assert.AreEqual(mockOrderInfoEntity.TimeZone, task.Value.TimeZone);
            Assert.AreEqual(mockOrderInfoEntity.IsOrderBalanceEnabled, task.Value.IsOrderBalanceEnabled);
            Assert.AreEqual(mockOrderInfoEntity.FormattedPhone, task.Value.FormattedPhone);
            Assert.AreEqual(mockOrderInfoEntity.InsuranceCarrier, task.Value.InsuranceCarrier);
            Assert.AreEqual(mockOrderInfoEntity.InsurancePlan, task.Value.InsurancePlan);
            Assert.AreEqual(mockOrderInfoEntity.PolicyNumber, task.Value.PolicyNumber);
            Assert.IsTrue(task.IsSuccess);
            Assert.IsNotNull(task.Value);
        }
       

        [TestMethod]
        public async Task TestGetOrderHeaderOrderInfoQuery_Return_Failure()
        {
            long clientGuid = 14800200;
            long visitGuid = 1234;
            long userGuid = 5678;   
            string repoError = "Repository failed to fetch data";

            _repository.Setup(r => r.GetOrderHeaderOrderInfo(clientGuid, visitGuid, userGuid))
                .ReturnsAsync(new Result<OrderHeaderOrderInfoEntity>()
                    .WithError(repoError));

            var handler = new OrderHeaderOrderInfoQueryHandler(
                _repository.Object, _mapper);

            var query = new OrderHeaderOrderInfoQuery
            {
                ClientGuid = clientGuid,
                VisitGuid = visitGuid,
                UserGuid = userGuid
            };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Errors.Any());

            var actualMessage = result.Errors.First().Message;

            Assert.IsTrue(
                actualMessage.StartsWith("Order Header Common Order Info Errors -"),
                $"Unexpected error prefix: {actualMessage}");

            Assert.IsTrue(
                actualMessage.Contains(repoError),
                $"Expected repo error not found. Actual: {actualMessage}");
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