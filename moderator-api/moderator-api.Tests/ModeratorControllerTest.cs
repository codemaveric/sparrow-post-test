using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using moderator_api.Controllers;
using moderator_api.Core;
using moderator_api.Core.Models;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace moderator_api.Tests
{
    public class ModeratorControllerTest
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
           
        }

        [Test]
        public async Task WillReturnBadRequestIfFormCollectionIsNull()
        {
            // arrange
            var mockCognitiveClient = new Mock<ICognitiveClient>();
            
            ModeratorController moderatorController = new ModeratorController(Mock.Of<ILogger<ModeratorController>>(), mockCognitiveClient.Object);

            var actionResult = await moderatorController.Post(null);

            var badResult = actionResult as BadRequestObjectResult;

            Assert.IsNotNull(badResult);

            Assert.AreEqual(400, badResult.StatusCode);
            Assert.AreEqual(ErrorMessagesModel.IMAGE_TEXT_REQUIRED, badResult.Value);
        }

        [Test]
        public async Task WillReturnBadRequestIfFileIsNotImage()
        {
            // arrange
            var mockCognitiveClient = new Mock<ICognitiveClient>();

            ModeratorController moderatorController = new ModeratorController(Mock.Of<ILogger<ModeratorController>>(), mockCognitiveClient.Object);
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy image file")), 0, 0, "Data", "dummy.exe");
            
            var formData = new Mock<IFormCollection>();

            formData.Setup(c => c.Files["uploadFile"]).Returns(file);
            formData.Setup(c => c["htmlText"]).Returns("HTML Text");
            formData.Setup(c => c.Files.Count).Returns(1);

            var actionResult = await moderatorController.Post(formData.Object);

            var badResult = actionResult as BadRequestObjectResult;

            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);
            Assert.AreEqual(ErrorMessagesModel.FILE_NOT_IMAGE, badResult.Value);
        }


        [Test]
        public async Task WillReturnBadRequestIfHtmlIsGreaterThan1024Chars()
        {
            // arrange
            var mockCognitiveClient = new Mock<ICognitiveClient>();

            ModeratorController moderatorController = new ModeratorController(Mock.Of<ILogger<ModeratorController>>(), mockCognitiveClient.Object);
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy image file")), 0, 0, "Data", "dummy.exe");

            var formData = new Mock<IFormCollection>();

            formData.Setup(c => c.Files["uploadFile"]).Returns(file);
            formData.Setup(c => c["htmlText"]).Returns(@"djkashjb hjcksbjdhmasb chjshcb shdjyhs mamjh,vsdbk vuysdjhasuyjvhms hkujhvbsuvjmhsdnv
sduyvjhdms ujb djkashjb hjcksbjdhmasb chjshcb shdjyhs mamjh,vsdbk vuysdjhasuyjvhms hkujhvbsuvjmhsdnv sduyvjhdms ujb djkashjb hjcksbjdhmasb chjshcb
shdjyhs mamjh,vsdbk vuysdjhasuyjvhms hkujhvbsuvjmhsdnv sduyvjhdms ujb djkashjb hjcksbjdhmasb chjshcb shdjyhs mamjh,vsdbk vuysdjhasuyjvhms hkujhvbsuvjmhsdnv
sduyvjhdms ujb djkashjb hjcksbjdhmasb chjshcb shdjyhs mamjh,vsdbk vuysdjhasuyjvhms hkujhvbsuvjmhsdnv sduyvjhdms ujb djkashjb hjcksbjdhmasb chjshcb shdjyhs
mamjh,vsdbk vuysdjhasuyjvhms hkujhvbsuvjmhsdnv sduyvjhdms ujb djkashjb hjcksbjdhmasb chjshcb shdjyhs mamjh,vsdbk vuysdjhasuyjvhms hkujhvbsuvjmhsdnv sduyvjhdms
ujb djkashjb hjcksbjdhmasb chjshcb shdjyhs mamjh,vsdbk vuysdjhasuyjvhms hkujhvbsuvjmhsdnv sduyvjhdms ujb djkashjb hjcksbjdhmasb chjshcb shdjyhs mamjh,vsdbk
vuysdjhasuyjvhms hkujhvbsuvjmhsdnv sduyvjhdms ujb djkashjb hjcksbjdhmasb chjshcb shdjyhs mamjh,vsdbk vuysdjhasuyjvhms hkujhvbsuvjmhsdnv sduyvjhdms ujb djkashjb hjcksh");
            formData.Setup(c => c.Files.Count).Returns(1);

            var actionResult = await moderatorController.Post(formData.Object);

            var badResult = actionResult as BadRequestObjectResult;

            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);
            Assert.AreEqual(ErrorMessagesModel.HTML_TEXT_TOO_LONG, badResult.Value);
        }


        [Test]
        public async Task WillReturnBadRequestWhenUnableToProcessImage()
        {
            // arrange
            ModerateTextResponseModel moderateText = new ModerateTextResponseModel
            {
                Status = new Status
                {
                    Code = 3000
                }
            };

            ModerateImageResponseModel moderateImage = null;
            var mockCognitiveClient = new Mock<ICognitiveClient>();
            mockCognitiveClient.Setup(c => c.ModerateTextAsync(It.IsAny<string>())).ReturnsAsync((moderateText, TimeSpan.FromMilliseconds(100)));

            mockCognitiveClient.Setup(c => c.ModerateImageAsync(It.IsAny<byte[]>(), It.IsAny<string>())).ReturnsAsync((moderateImage, TimeSpan.FromMilliseconds(100)));

            ModeratorController moderatorController = new ModeratorController(Mock.Of<ILogger<ModeratorController>>(), mockCognitiveClient.Object);
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy image file")), 0, 0, "Data", "dummy.jpg");

            var formData = new Mock<IFormCollection>();

            formData.Setup(c => c.Files["uploadFile"]).Returns(file);
            formData.Setup(c => c["htmlText"]).Returns(@"Hello world");
            formData.Setup(c => c.Files.Count).Returns(1);

            var actionResult = await moderatorController.Post(formData.Object);

            var badResult = actionResult as BadRequestObjectResult;

            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);
            Assert.AreEqual(ErrorMessagesModel.UNABLE_TO_PROCESS_IMAGE_REQUEST, badResult.Value);
        }

        [Test]
        public async Task WillReturnBadRequestWhenUnableToProcessText()
        {
            // arrange
            ModerateTextResponseModel moderateText = null;

            ModerateImageResponseModel moderateImage = null;
            var mockCognitiveClient = new Mock<ICognitiveClient>();
            mockCognitiveClient.Setup(c => c.ModerateTextAsync(It.IsAny<string>())).ReturnsAsync((moderateText, TimeSpan.FromMilliseconds(100)));

            mockCognitiveClient.Setup(c => c.ModerateImageAsync(It.IsAny<byte[]>(), It.IsAny<string>())).ReturnsAsync((moderateImage, TimeSpan.FromMilliseconds(100)));

            ModeratorController moderatorController = new ModeratorController(Mock.Of<ILogger<ModeratorController>>(), mockCognitiveClient.Object);
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy image file")), 0, 0, "Data", "dummy.jpg");

            var formData = new Mock<IFormCollection>();

            formData.Setup(c => c.Files["uploadFile"]).Returns(file);
            formData.Setup(c => c["htmlText"]).Returns(@"Hello world");
            formData.Setup(c => c.Files.Count).Returns(1);

            var actionResult = await moderatorController.Post(formData.Object);

            var badResult = actionResult as BadRequestObjectResult;

            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult.StatusCode);
            Assert.AreEqual(ErrorMessagesModel.UNABLE_TO_PROCESS_TEXT_REQUEST, badResult.Value);
        }

        [Test]
        public async Task WillReturnOkResultIfRequestIsValid()
        {
            // arrange
            ModerateTextResponseModel moderateText = new ModerateTextResponseModel
            {
                Status = new Status
                {
                    Code = 3000
                }
            };

            ModerateImageResponseModel moderateImage = new ModerateImageResponseModel {
                Status = new ModerateStatus
                {
                    Code = 3000
                }
            };
            var mockCognitiveClient = new Mock<ICognitiveClient>();
            mockCognitiveClient.Setup(c => c.ModerateTextAsync(It.IsAny<string>())).ReturnsAsync((moderateText, TimeSpan.FromMilliseconds(100)));

            mockCognitiveClient.Setup(c => c.ModerateImageAsync(It.IsAny<byte[]>(), It.IsAny<string>())).ReturnsAsync((moderateImage, TimeSpan.FromMilliseconds(100)));

            ModeratorController moderatorController = new ModeratorController(Mock.Of<ILogger<ModeratorController>>(), mockCognitiveClient.Object);
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy image file")), 0, 0, "Data", "dummy.jpg");

            var formData = new Mock<IFormCollection>();

            formData.Setup(c => c.Files["uploadFile"]).Returns(file);
            formData.Setup(c => c["htmlText"]).Returns(@"Hello world");
            formData.Setup(c => c.Files.Count).Returns(1);

            var actionResult = await moderatorController.Post(formData.Object);

            var okResult = actionResult as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}
