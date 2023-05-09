using Castle.Core.Configuration;
using ERAEntities.DataModels;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using RoadsideAssistanceBusiness.Interfaces;
using RoadsideAssistanceBusiness.Models;
using RoadsideAssistanceBusiness.Services;
using RoadsideAssistanceBusinessTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadsideAssistanceBusiness.Services.Tests
{
    [TestClass()]
    public class RoadsideAssistanceServiceTests
    {

        [TestMethod()]
        //Assert
        [ExpectedException(typeof(ArgumentNullException), "Invalid location data")]
        public async Task FindNearestAssistantsTest_InvlaidGelocation_null_ShouldThrow_ArgumentNullException()
        {
            //Arrange
            var context = new TestContext<RoadsideAssistanceService>();
            var roadsideService = SetUpService(context);

            //Act
            var res = await roadsideService.FindNearestAssistants(null, 1);
        }

        [TestMethod()]
        //Assert
        [ExpectedException(typeof(ArgumentNullException), "Invalid location data")]
        public async Task FindNearestAssistantsTest_InvlaidGelocation_Longitude_ShouldThrow_ArgumentNullException()
        {
            //Arrange
            var geolocation  = new Geolocation() { Longitude = 200, Lattitude= 20 };
            var context = new TestContext<RoadsideAssistanceService>();
            var roadsideService = SetUpService(context);

            //Act
            var res = await roadsideService.FindNearestAssistants(geolocation, 1);
        }

        [TestMethod()]
        //Assert
        [ExpectedException(typeof(ArgumentNullException), "Invalid location data")]
        public async Task FindNearestAssistantsTest_InvlaidGelocation_Latitude_ShouldThrow_ArgumentNullException()
        {
            //Arrange
            var geolocation = new Geolocation() { Longitude = 20, Lattitude = 200 };
            var context = new TestContext<RoadsideAssistanceService>();
            var roadsideService = SetUpService(context);

            //Act
            var res = await roadsideService.FindNearestAssistants(geolocation, 1);
        }

        [TestMethod()]
        public async Task FindNearestAssistantsTest_ValidRequest_ShouldSucceed()
        {
            //Arrange
            var geolocation = new Geolocation() { Longitude = -60, Lattitude = 40 };
            
            var mockEntities = TestUtilities.GetERAEntitiesMock();
            var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var assistantDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Assistant>()
            {
                new ERAEntities.DataModels.Assistant() { Id = 1, ServiceProviderName = "aaa", Address = "Adddress1", Zipcode= 1234 },
                new ERAEntities.DataModels.Assistant() { Id = 2, ServiceProviderName = "bbb", Address = "Adddress2", Zipcode= 2222 },
                new ERAEntities.DataModels.Assistant() { Id = 3, ServiceProviderName = "ccc", Address = "Adddress3", Zipcode= 3333 },
                new ERAEntities.DataModels.Assistant() { Id = 4, ServiceProviderName = "ddd", Address = "Adddress4", Zipcode= 4444 },
                new ERAEntities.DataModels.Assistant() { Id = 5, ServiceProviderName = "eee", Address = "Adddress5", Zipcode= 6666 }

            }.AsEnumerable());
            
            mockEntities.MockPropertyCall(assistantDbSet);
            var assistantLocationssDbSet = TestUtilities.SetUpMockDbSet( new List<AssistantLocation>()
            { 
                new AssistantLocation() { AssistantId = 1, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 80)), Status = 0 },
                new AssistantLocation() { AssistantId = 2, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 80)), Status = 0 },
                new AssistantLocation() { AssistantId = 3, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 70)), Status = 0 },
                new AssistantLocation() { AssistantId = 4, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 20)), Status = 0 },
                new AssistantLocation() { AssistantId = 5, Geolocation = geoFactory.CreatePoint(new Coordinate(-55, 40)), Status = 1 }

            }.AsEnumerable());
            mockEntities.MockPropertyCall(assistantLocationssDbSet);

            var context = new TestContext<RoadsideAssistanceService>(mockEntities);
            var roadsideService = SetUpService(context);

            //Act
            var res = await roadsideService.FindNearestAssistants(geolocation, 5);

            //Assert
            Assert.AreEqual(4, res.Count);
        }

        [TestMethod()]
        public async Task FindNearestAssistantsTest_ValidRequest_Limit1_ShouldSucceed()
        {
            //Arrange
            var geolocation = new Geolocation() { Longitude = -60, Lattitude = 40 };

            var mockEntities = TestUtilities.GetERAEntitiesMock();
            var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var assistantDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Assistant>()
            {
                new ERAEntities.DataModels.Assistant() { Id = 1, ServiceProviderName = "aaa", Address = "Adddress1", Zipcode= 1234 },
                new ERAEntities.DataModels.Assistant() { Id = 2, ServiceProviderName = "bbb", Address = "Adddress2", Zipcode= 2222 },
                new ERAEntities.DataModels.Assistant() { Id = 3, ServiceProviderName = "ccc", Address = "Adddress3", Zipcode= 3333 },
                new ERAEntities.DataModels.Assistant() { Id = 4, ServiceProviderName = "ddd", Address = "Adddress4", Zipcode= 4444 },
                new ERAEntities.DataModels.Assistant() { Id = 5, ServiceProviderName = "eee", Address = "Adddress5", Zipcode= 6666 }

            }.AsEnumerable());

            mockEntities.MockPropertyCall(assistantDbSet);
            var assistantLocationssDbSet = TestUtilities.SetUpMockDbSet(new List<AssistantLocation>()
            {
                new AssistantLocation() { AssistantId = 1, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 80)), Status = 0 },
                new AssistantLocation() { AssistantId = 2, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 80)), Status = 0 },
                new AssistantLocation() { AssistantId = 3, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 70)), Status = 0 },
                new AssistantLocation() { AssistantId = 4, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 20)), Status = 0 },
                new AssistantLocation() { AssistantId = 5, Geolocation = geoFactory.CreatePoint(new Coordinate(-55, 40)), Status = 0 }

            }.AsEnumerable());
            mockEntities.MockPropertyCall(assistantLocationssDbSet);

            var context = new TestContext<RoadsideAssistanceService>(mockEntities);
            var roadsideService = SetUpService(context);

            //Act
            var res = await roadsideService.FindNearestAssistants(geolocation, 1);

            //Assert
            Assert.AreEqual(1, res.Count);
        }

        [TestMethod()]
        public async Task FindNearestAssistantsTest_ValidRequest_NoAvailableAssistants_ShouldSucceed()
        {
            //Arrange
            var geolocation = new Geolocation() { Longitude = -60, Lattitude = 40 };

            var mockEntities = TestUtilities.GetERAEntitiesMock();
            var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var assistantDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Assistant>()
            {
                new ERAEntities.DataModels.Assistant() { Id = 1, ServiceProviderName = "aaa", Address = "Adddress1", Zipcode= 1234 },
                new ERAEntities.DataModels.Assistant() { Id = 2, ServiceProviderName = "bbb", Address = "Adddress2", Zipcode= 2222 },
                new ERAEntities.DataModels.Assistant() { Id = 3, ServiceProviderName = "ccc", Address = "Adddress3", Zipcode= 3333 },
                new ERAEntities.DataModels.Assistant() { Id = 4, ServiceProviderName = "ddd", Address = "Adddress4", Zipcode= 4444 },
                new ERAEntities.DataModels.Assistant() { Id = 5, ServiceProviderName = "eee", Address = "Adddress5", Zipcode= 6666 }

            }.AsEnumerable());

            mockEntities.MockPropertyCall(assistantDbSet);
            var assistantLocationssDbSet = TestUtilities.SetUpMockDbSet(new List<AssistantLocation>()
            {
                new AssistantLocation() { AssistantId = 1, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 80)), Status = 1},
                new AssistantLocation() { AssistantId = 2, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 80)), Status = 1},
                new AssistantLocation() { AssistantId = 3, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 70)), Status = 1},
                new AssistantLocation() { AssistantId = 4, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 20)), Status = 1},
                new AssistantLocation() { AssistantId = 5, Geolocation = geoFactory.CreatePoint(new Coordinate(-55, 40)), Status = 1}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(assistantLocationssDbSet);

            var context = new TestContext<RoadsideAssistanceService>(mockEntities);
            var roadsideService = SetUpService(context);

            //Act
            var res = await roadsideService.FindNearestAssistants(geolocation, 0);

            //Assert
            Assert.AreEqual(0, res.Count);
        }

        [TestMethod()]
        //Assert
        [ExpectedException(typeof(ArgumentNullException), "Customer or location details")]
        public async Task ReserveAssistantTest_InvalidCustomer_ShouldThrow_ArgumentNullException()
        {
            //Arrange
            var context = new TestContext<RoadsideAssistanceService>();
            var roadsideService = SetUpService(context);

            //Act
            var res = await roadsideService.ReserveAssistant(null, new Geolocation { Lattitude = 40, Longitude = -60 });
        }

        [TestMethod()]
        //Assert
        [ExpectedException(typeof(ArgumentNullException), "Customer or location details")]
        public async Task ReserveAssistantTest_InvalidCustomerLocation_ShouldThrow_ArgumentNullException()
        {
            //Arrange
            var context = new TestContext<RoadsideAssistanceService>();
            var roadsideService = SetUpService(context);
            
            //Act
            var res = await roadsideService.ReserveAssistant(new Models.Customer { Id = 1, Name = "test"}, new Geolocation { Lattitude = 200, Longitude = -60 });
        }

        [TestMethod()]       
        public async Task ReserveAssistantTest_Valid_ShouldSucceed()
        {
            //Arrange
            var geolocation = new Geolocation() { Longitude = -60, Lattitude = 40 };

            var mockEntities = TestUtilities.GetERAEntitiesMock();
            var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var assistantDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Assistant>()
            {
                new ERAEntities.DataModels.Assistant() { Id = 1, ServiceProviderName = "aaa", Address = "Adddress1", Zipcode= 1234 },
                new ERAEntities.DataModels.Assistant() { Id = 2, ServiceProviderName = "bbb", Address = "Adddress2", Zipcode= 2222 },
                new ERAEntities.DataModels.Assistant() { Id = 3, ServiceProviderName = "ccc", Address = "Adddress3", Zipcode= 3333 },
                new ERAEntities.DataModels.Assistant() { Id = 4, ServiceProviderName = "ddd", Address = "Adddress4", Zipcode= 4444 },
                new ERAEntities.DataModels.Assistant() { Id = 5, ServiceProviderName = "eee", Address = "Adddress5", Zipcode= 6666 }

            }.AsEnumerable());

            mockEntities.MockPropertyCall(assistantDbSet);
            var assistantLocationssDbSet = TestUtilities.SetUpMockDbSet(new List<AssistantLocation>()
            {
                new AssistantLocation() { AssistantId = 1, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 80)), Status = 0},
                new AssistantLocation() { AssistantId = 2, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 80)), Status = 0},
                new AssistantLocation() { AssistantId = 3, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 70)), Status = 0},
                new AssistantLocation() { AssistantId = 4, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 20)), Status = 0},
                new AssistantLocation() { AssistantId = 5, Geolocation = geoFactory.CreatePoint(new Coordinate(-55, 40)), Status = 0}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(assistantLocationssDbSet);

            var customersDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Customer>()
            {
                new ERAEntities.DataModels.Customer() { Id = 1},
                new ERAEntities.DataModels.Customer() { Id = 2},
                new ERAEntities.DataModels.Customer() { Id = 3},
                new ERAEntities.DataModels.Customer() { Id = 4},
                new ERAEntities.DataModels.Customer() { Id = 5}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(customersDbSet);

            var context = new TestContext<RoadsideAssistanceService>(mockEntities);
            var roadsideService = SetUpService(context);

            // Act
            var res = await roadsideService.ReserveAssistant(new Models.Customer { Id = 2}, new Geolocation { Longitude=-81, Lattitude=22});

            //Assert
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Id > 0);
        }

        [TestMethod()]
        //Assert
        [ExpectedException(typeof(Exception), "The customer have an active resrvation")]
        public async Task ReserveAssistantTest_CustomerAlreadyHaveResrvation_ShouldThrow_Exception()
        {
            //Arrange
            var geolocation = new Geolocation() { Longitude = -60, Lattitude = 40 };

            var mockEntities = TestUtilities.GetERAEntitiesMock();
            var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var assistantDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Assistant>()
            {
                new ERAEntities.DataModels.Assistant() { Id = 1, ServiceProviderName = "aaa", Address = "Adddress1", Zipcode= 1234 },
                new ERAEntities.DataModels.Assistant() { Id = 2, ServiceProviderName = "bbb", Address = "Adddress2", Zipcode= 2222 },
                new ERAEntities.DataModels.Assistant() { Id = 3, ServiceProviderName = "ccc", Address = "Adddress3", Zipcode= 3333 },
                new ERAEntities.DataModels.Assistant() { Id = 4, ServiceProviderName = "ddd", Address = "Adddress4", Zipcode= 4444 },
                new ERAEntities.DataModels.Assistant() { Id = 5, ServiceProviderName = "eee", Address = "Adddress5", Zipcode= 6666 }

            }.AsEnumerable());

            mockEntities.MockPropertyCall(assistantDbSet);
            var assistantLocationssDbSet = TestUtilities.SetUpMockDbSet(new List<AssistantLocation>()
            {
                new AssistantLocation() { AssistantId = 1, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 80)), Status = 0},
                new AssistantLocation() { AssistantId = 2, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 80)), Status = 0},
                new AssistantLocation() { AssistantId = 3, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 70)), Status = 0},
                new AssistantLocation() { AssistantId = 4, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 20)), Status = 0},
                new AssistantLocation() { AssistantId = 5, Geolocation = geoFactory.CreatePoint(new Coordinate(-55, 40)), Status = 0}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(assistantLocationssDbSet);

            var customersDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Customer>()
            {
                new ERAEntities.DataModels.Customer() { Id = 1},
                new ERAEntities.DataModels.Customer() { Id = 2},
                new ERAEntities.DataModels.Customer() { Id = 3},
                new ERAEntities.DataModels.Customer() { Id = 4},
                new ERAEntities.DataModels.Customer() { Id = 5}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(customersDbSet);

            var reservationsDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.CustomerAssistantAssignment>()
            {
                new ERAEntities.DataModels.CustomerAssistantAssignment() { Id = 1, CustomerId = 2, AssitantId = 4}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(reservationsDbSet);

            var context = new TestContext<RoadsideAssistanceService>(mockEntities);
            var roadsideService = SetUpService(context);

            //Act
            var res = await roadsideService.ReserveAssistant(new Models.Customer { Id = 2 }, new Geolocation { Longitude = -81, Lattitude = 22 });

        }

        [TestMethod()]
        public async Task ReserveAssistantTest_valid_shouldSucceed()
        {
            //Arrange
            var geolocation = new Geolocation() { Longitude = -60, Lattitude = 40 };

            var mockEntities = TestUtilities.GetERAEntitiesMock();
            var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var assistantDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Assistant>()
            {
                new ERAEntities.DataModels.Assistant() { Id = 1, ServiceProviderName = "aaa", Address = "Adddress1", Zipcode= 1234 },
                new ERAEntities.DataModels.Assistant() { Id = 2, ServiceProviderName = "bbb", Address = "Adddress2", Zipcode= 2222 },
                new ERAEntities.DataModels.Assistant() { Id = 3, ServiceProviderName = "ccc", Address = "Adddress3", Zipcode= 3333 },
                new ERAEntities.DataModels.Assistant() { Id = 4, ServiceProviderName = "ddd", Address = "Adddress4", Zipcode= 4444 },
                new ERAEntities.DataModels.Assistant() { Id = 5, ServiceProviderName = "eee", Address = "Adddress5", Zipcode= 6666 }

            }.AsEnumerable());

            mockEntities.MockPropertyCall(assistantDbSet);
            var assistantLocationssDbSet = TestUtilities.SetUpMockDbSet(new List<AssistantLocation>()
            {
                new AssistantLocation() {Id = 1, AssistantId = 1, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 80)), Status = 0},
                new AssistantLocation() {Id = 2, AssistantId = 2, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 80)), Status = 0},
                new AssistantLocation() {Id = 3, AssistantId = 3, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 70)), Status = 0},
                new AssistantLocation() {Id = 4, AssistantId = 4, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 20)), Status = 0},
                new AssistantLocation() {Id = 5, AssistantId = 5, Geolocation = geoFactory.CreatePoint(new Coordinate(-55, 40)), Status = 0}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(assistantLocationssDbSet);


            var customersDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Customer>()
            {
                new ERAEntities.DataModels.Customer() { Id = 1},
                new ERAEntities.DataModels.Customer() { Id = 2},
                new ERAEntities.DataModels.Customer() { Id = 3},
                new ERAEntities.DataModels.Customer() { Id = 4},
                new ERAEntities.DataModels.Customer() { Id = 5}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(customersDbSet);

            var context = new TestContext<RoadsideAssistanceService>(mockEntities);
            var roadsideService = SetUpService(context);
            //Act
            await roadsideService.ReserveAssistant(new Models.Customer { Id = 3 }, geolocation);
            var res = await roadsideService.FindNearestAssistants(new Geolocation { Longitude = -80, Lattitude = 80 }, 10);

            //Assert
            Assert.AreEqual(4, res.Count);

        }

        [TestMethod()]
        public async Task ReleaseAssistantTest_valid_shouldSucceed()
        {
            //Arrange
            var geolocation = new Geolocation() { Longitude = -60, Lattitude = 40 };

            var mockEntities = TestUtilities.GetERAEntitiesMock();
            var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var assistantDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Assistant>()
            {
                new ERAEntities.DataModels.Assistant() { Id = 1, ServiceProviderName = "aaa", Address = "Adddress1", Zipcode= 1234 },
                new ERAEntities.DataModels.Assistant() { Id = 2, ServiceProviderName = "bbb", Address = "Adddress2", Zipcode= 2222 },
                new ERAEntities.DataModels.Assistant() { Id = 3, ServiceProviderName = "ccc", Address = "Adddress3", Zipcode= 3333 },
                new ERAEntities.DataModels.Assistant() { Id = 4, ServiceProviderName = "ddd", Address = "Adddress4", Zipcode= 4444 },
                new ERAEntities.DataModels.Assistant() { Id = 5, ServiceProviderName = "eee", Address = "Adddress5", Zipcode= 6666 }

            }.AsEnumerable());

            mockEntities.MockPropertyCall(assistantDbSet);
            var assistantLocationssDbSet = TestUtilities.SetUpMockDbSet(new List<AssistantLocation>()
            {
                new AssistantLocation() {Id = 1, AssistantId = 1, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 80)), Status = 0},
                new AssistantLocation() {Id = 2, AssistantId = 2, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 80)), Status = 0},
                new AssistantLocation() {Id = 3, AssistantId = 3, Geolocation = geoFactory.CreatePoint(new Coordinate(-60, 70)), Status = 0},
                new AssistantLocation() {Id = 4, AssistantId = 4, Geolocation = geoFactory.CreatePoint(new Coordinate(-80, 20)), Status = 0},
                new AssistantLocation() {Id = 5, AssistantId = 5, Geolocation = geoFactory.CreatePoint(new Coordinate(-55, 40)), Status = 1}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(assistantLocationssDbSet);


            var customersDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.Customer>()
            {
                new ERAEntities.DataModels.Customer() { Id = 1},
                new ERAEntities.DataModels.Customer() { Id = 2},
                new ERAEntities.DataModels.Customer() { Id = 3},
                new ERAEntities.DataModels.Customer() { Id = 4},
                new ERAEntities.DataModels.Customer() { Id = 5}

            }.AsEnumerable());
            mockEntities.MockPropertyCall(customersDbSet);

            var reservationsDbSet = TestUtilities.SetUpMockDbSet(new List<ERAEntities.DataModels.CustomerAssistantAssignment>()
            {
                new ERAEntities.DataModels.CustomerAssistantAssignment() { Id = 1, AssitantId = 5, CustomerId = 5, StartTime = DateTime.UtcNow.AddHours(-4)}
               
            }.AsEnumerable());
            mockEntities.MockPropertyCall(reservationsDbSet);

            var context = new TestContext<RoadsideAssistanceService>(mockEntities);
            var roadsideService = SetUpService(context);

            //Act
            await roadsideService.ReleaseAssistant(new Models.Customer { Id = 5}, new Models.Assistant { Id=5});
            var res = await roadsideService.FindNearestAssistants(new Geolocation { Longitude = -80, Lattitude = 80 }, 10);

            //Assert
            Assert.AreEqual(5, res.Count);

        }


        public RoadsideAssistanceService SetUpService(TestContext<RoadsideAssistanceService> context)
        {
            return new RoadsideAssistanceService(context.ConfigurationMock.Object, context.EntityFactoryMock.Object);
        }
    }

    public class TestContext<T> : IDisposable
    {
        private bool _disposed;

        public Mock<Microsoft.Extensions.Configuration.IConfiguration> ConfigurationMock { get; set; }
        public Mock<IEntityFactory> EntityFactoryMock { get; set; }
        public TestContext()
        {
            var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configMock.Setup(c => c.GetSection(It.IsAny<String>())).Returns(new Mock<IConfigurationSection>().Object);
            ConfigurationMock = configMock;
             
            var entityMock =  new Mock<IEntityFactory>();
            var entities = TestUtilities.GetERAEntitiesMock().Object;
            entityMock.Setup(x => x.CreateDbConext(It.IsAny<string>())).Returns(entities);

            EntityFactoryMock = entityMock;

        }

        public TestContext(Mock<ERAEntities.ERAContext> mockEntities)
        {
            var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configMock.Setup(c => c.GetSection(It.IsAny<String>())).Returns(new Mock<IConfigurationSection>().Object);
            ConfigurationMock = configMock;

            var entityMock = new Mock<IEntityFactory>();
            var entities = mockEntities.Object;
            entityMock.Setup(x => x.CreateDbConext(It.IsAny<string>())).Returns(entities);

            EntityFactoryMock = entityMock;

        }
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ConfigurationMock = null;
                    EntityFactoryMock = null;
                }
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}