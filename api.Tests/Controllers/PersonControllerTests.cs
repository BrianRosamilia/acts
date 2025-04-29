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
    public class PersonControllerTests
    {
        private HttpClient _httpClient;
        private Mock<IRequestHandler<CreateLog, CreateLogResult>> mockCreateLogHandler;

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
        public async Task GetAllPersons_ReturnsSuccessStatusCode()
        {
            var response = await _httpClient.GetAsync("/person");

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            mockCreateLogHandler.Verify(mock => mock.Handle(It.Is<CreateLog>(log => log.LogLevel == "Information"),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task GetPersonByName_WhenPersonExists_ReturnsCorrectPerson()
        {
            var newPerson = "Abigail";

            await _httpClient.PostAsJsonAsync("/person", "Abigail");

            var response = await _httpClient.GetAsync($"/person/{newPerson}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var person = await response.Content.ReadFromJsonAsync<GetPersonByNameResult>();
            Assert.AreEqual(newPerson, person.Person.Name);
        }

        [TestMethod]
        public async Task CreatePerson_WithValidData_ReturnsCreatedStatusCode()
        {
            var response = await _httpClient.PostAsJsonAsync("/person", "Abigail");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task CreatePerson_WithDuplicateName_ReturnsBadRequest()
        {
            string personName = "DuplicatePerson";

            await _httpClient.PostAsJsonAsync("/person", personName);
            var duplicateResponse = await _httpClient.PostAsJsonAsync("/person", personName);

            Assert.AreEqual(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);

            mockCreateLogHandler.Verify(
                mock => mock.Handle(
                    It.Is<CreateLog>(log => log.LogLevel == "Warning"),
                    It.IsAny<CancellationToken>()),
                Times.Once());

        }
    }
}