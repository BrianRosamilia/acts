using System.Net;
using System.Net.Http.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;

namespace StargateAPI.Tests
{
    [TestClass]
    public class AstronautDutyControllerTests
    {
        private HttpClient _httpClient = null!;
        private Mock<IRequestHandler<CreateLog, CreateLogResult>> mockCreateLogHandler = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            mockCreateLogHandler = new Mock<IRequestHandler<CreateLog, CreateLogResult>>();
            mockCreateLogHandler.Setup(x => x.Handle(
                It.IsAny<CreateLog>(),
                It.IsAny<CancellationToken>()));

            var server = Setup.Get()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddMediatR(cfg =>
                        {
                            cfg.AddRequestPreProcessor<StargateAPI.Business.Commands.CreateAstronautDutyPreProcessor>();
                            cfg.AddRequestPreProcessor<StargateAPI.Business.Commands.CreatePersonPreProcessor>();
                            cfg.RegisterServicesFromAssemblies(typeof(StargateAPI.Business.Data.StargateContext).Assembly);
                        });

                        services.AddTransient(provider => mockCreateLogHandler.Object);
                    });
                });

            _httpClient = server.CreateClient();
        }

        [TestMethod]
        public async Task GetAstronautDutiesByName_ReturnsSuccessStatusCode()
        {
            await _httpClient.PostAsJsonAsync("/person", "Abigail");

            var dutyRequest = new CreateAstronautDuty
            {
                Name = "Abigail",
                DutyTitle = "Admiral",
                Rank = "SG1"
            };
            await _httpClient.PostAsJsonAsync("/astronautduty", dutyRequest);

            var response = await _httpClient.GetAsync("/astronautduty/Abigail");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            mockCreateLogHandler.Verify(mock => mock.Handle(
                It.Is<CreateLog>(log => log.LogLevel == "Information"),
                It.IsAny<CancellationToken>()),
                Times.AtLeastOnce());
        }

        [TestMethod]
        public async Task GetAstronautDutiesByName_WhenPersonExists_ReturnsCorrectDuties()
        {
            string personName = "Abigail";
            await _httpClient.PostAsJsonAsync("/person", personName);

            var dutyRequest = new CreateAstronautDuty
            {
                Name = personName,
                DutyTitle = "Admiral",
                Rank = "SG1"
            };
            await _httpClient.PostAsJsonAsync("/astronautduty", dutyRequest);

            var response = await _httpClient.GetAsync($"/astronautduty/{personName}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<GetAstronautDutiesByNameResult>();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AstronautDuties);
            Assert.IsTrue(result.AstronautDuties.Any());
            var duty = result.AstronautDuties.First();
            Assert.AreEqual("Admiral", duty.DutyTitle);
            Assert.AreEqual("SG1", duty.Rank);
        }

        [TestMethod]
        public async Task CreateAstronautDuty_WithValidData_ReturnsSuccessStatusCode()
        {
            string personName = "Abigail";
            await _httpClient.PostAsJsonAsync("/person", personName);

            var dutyRequest = new CreateAstronautDuty
            {
                Name = personName,
                DutyTitle = "Admiral",
                Rank = "SG1"
            };
            var response = await _httpClient.PostAsJsonAsync("/astronautduty", dutyRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            mockCreateLogHandler.Verify(mock => mock.Handle(
                It.Is<CreateLog>(log => log.LogLevel == "Information"),
                It.IsAny<CancellationToken>()),
                Times.Exactly(2)); //one for creation, one for duty
        }

        [TestMethod]
        public async Task CreateAstronautDuty_WithNonExistentPerson_ReturnsError()
        {
            var dutyRequest = new CreateAstronautDuty
            {
                Name = "NonExistentPerson",
                DutyTitle = "Admiral",
                Rank = "SG1"
            };
            var response = await _httpClient.PostAsJsonAsync("/astronautduty", dutyRequest);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

            mockCreateLogHandler.Verify(mock => mock.Handle(
                It.Is<CreateLog>(log => log.LogLevel == "Error"),
                It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task CreateAstronautDuty_WithDuplicateData_HandlesAppropriately()
        {
            string personName = "Abigail";
            await _httpClient.PostAsJsonAsync("/person", personName);

            var dutyRequest = new CreateAstronautDuty
            {
                Name = personName,
                DutyTitle = "Admiral",
                Rank = "SG1"
            };
            await _httpClient.PostAsJsonAsync("/astronautduty", dutyRequest);

            var duplicateResponse = await _httpClient.PostAsJsonAsync("/astronautduty", dutyRequest);

            Assert.IsTrue(duplicateResponse.StatusCode == HttpStatusCode.BadRequest);
        }
    }
}