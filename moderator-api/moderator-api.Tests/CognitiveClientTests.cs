using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using moderator_api.Core;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace moderator_api.Tests
{
    public class CognitiveClientTests
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Cognitive:URL", "https://url.test"},
                {"Cognitive:SubscriptionKey", "SectionValue"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Test]
        public void WillReturnNullIfModerateTextAPIFail()
        {

            var mockClientFactory = new Mock<IHttpClientFactory>();
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected().As<IHttpMessageHandler>()
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                });

            var client = new HttpClient(handler.Object);

            mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var logger = Mock.Of<ILogger<CognitiveClient>>();

            ICognitiveClient cognitiveClient = new CognitiveClient(logger, mockClientFactory.Object, _configuration);
            var response = cognitiveClient.ModerateTextAsync("HTML Text").Result;
            Assert.IsNull(response.moderateResp);
        }

        [Test]
        public void WillThrowArgumentExceptionWhenBodyIsNullorEmpty()
        {

            var mockClientFactory = new Mock<IHttpClientFactory>();
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected().As<IHttpMessageHandler>()
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                });

            var client = new HttpClient(handler.Object);

            mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var logger = Mock.Of<ILogger<CognitiveClient>>();

            ICognitiveClient cognitiveClient = new CognitiveClient(logger, mockClientFactory.Object, _configuration);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await cognitiveClient.ModerateTextAsync(string.Empty) );
        }

        [Test]
        public void WillReturnCognitiveTextModerationResponse()
        {

            var mockClientFactory = new Mock<IHttpClientFactory>();
            var handler = new Mock<HttpMessageHandler>();

            string stringResponse = "{ \"Status\": {\"Code\": 3000, \"Description\": \"OK\", \"Exception\": null }, \"TrackingId\": \"1717c837-cfb5-4fc0-9adc-24859bfd7fac\"}";

            handler.Protected().As<IHttpMessageHandler>()
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(stringResponse)
                });

            var client = new HttpClient(handler.Object);

            mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var logger = Mock.Of<ILogger<CognitiveClient>>();

            ICognitiveClient cognitiveClient = new CognitiveClient(logger, mockClientFactory.Object, _configuration);
            var response = cognitiveClient.ModerateTextAsync("HTML Text").Result;
            Assert.IsNotNull(response.moderateResp);
            Assert.AreEqual(response.moderateResp?.Status.Description, "OK");
        }

        [Test]
        public void WillThrowArgumentExceptionWhenImageBytesIsNull()
        {

            var mockClientFactory = new Mock<IHttpClientFactory>();
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected().As<IHttpMessageHandler>()
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                });

            var client = new HttpClient(handler.Object);

            mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var logger = Mock.Of<ILogger<CognitiveClient>>();

            ICognitiveClient cognitiveClient = new CognitiveClient(logger, mockClientFactory.Object, _configuration);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await cognitiveClient.ModerateImageAsync(null, "image/jpg"));
        }

        [Test]
        public void WillThrowArgumentExceptionWhenMediaTypeIsNullOrEmpty()
        {

            var mockClientFactory = new Mock<IHttpClientFactory>();
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected().As<IHttpMessageHandler>()
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                });

            var client = new HttpClient(handler.Object);

            mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var logger = Mock.Of<ILogger<CognitiveClient>>();

            ICognitiveClient cognitiveClient = new CognitiveClient(logger, mockClientFactory.Object, _configuration);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await cognitiveClient.ModerateImageAsync(Encoding.UTF8.GetBytes("Sparrow Image"), ""));
        }

        [Test]
        public void WillReturnNullIfModerateImageAPIFail()
        {

            var mockClientFactory = new Mock<IHttpClientFactory>();
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected().As<IHttpMessageHandler>()
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                });

            var client = new HttpClient(handler.Object);

            mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var logger = Mock.Of<ILogger<CognitiveClient>>();

            ICognitiveClient cognitiveClient = new CognitiveClient(logger, mockClientFactory.Object, _configuration);
            var response = cognitiveClient.ModerateImageAsync(Encoding.UTF8.GetBytes("Sparrow Image"), "image/jpeg").Result;
            Assert.IsNull(response.moderateResp);
        }
    }

    public interface IHttpMessageHandler
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
