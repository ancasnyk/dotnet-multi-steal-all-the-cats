using System.Net;
using System.Text.Json;
using CatStealer.Core.Configuration;
using CatStealer.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace CatStealer.Tests.Unit
{
    public class CatApiClientTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<CatApiClient>> _mockLogger;
        private readonly Mock<IOptions<CatsApiSettings>> _mockSettings;
        private readonly Mock<IOptions<JsonSerializerOptions>> _mockJsonOptions;

        public CatApiClientTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<CatApiClient>>();
            _mockSettings = new Mock<IOptions<CatsApiSettings>>();
            _mockJsonOptions = new Mock<IOptions<JsonSerializerOptions>>();

            _mockSettings.Setup(x => x.Value).Returns(new CatsApiSettings { ApiKey = "test-api-key", BaseUrl = "https://api.example.com" });
            _mockJsonOptions.Setup(x => x.Value).Returns(new JsonSerializerOptions());
        }

        [Fact]
        public async Task FetchCatsAsync_ShouldReturnCats_WhenApiCallIsSuccessful()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{\"id\":\"test1\",\"url\":\"https://example.com/cat.jpg\",\"width\":100,\"height\":100}]")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var catApiClient = new CatApiClient(_mockHttpClientFactory.Object, _mockLogger.Object, _mockSettings.Object, _mockJsonOptions.Object);

            var result = await catApiClient.FetchCatsAsync();

            Assert.Single(result);
            Assert.Equal("test1", result[0].Id);
        }

        [Fact]
        public async Task FetchImageAsync_ShouldReturnImageBytes_WhenApiCallIsSuccessful()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ByteArrayContent(new byte[] { 1, 2, 3, 4, 5 })
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var catApiClient = new CatApiClient(_mockHttpClientFactory.Object, _mockLogger.Object, _mockSettings.Object, _mockJsonOptions.Object);

            var result = await catApiClient.FetchImageAsync("https://example.com/cat.jpg");

            Assert.Equal(5, result.Length);
        }
    }
}
