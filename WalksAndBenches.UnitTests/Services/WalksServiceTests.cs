using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalksAndBenches.Models;
using WalksAndBenches.Services;

namespace WalksAndBenches.UnitTests.Services
{
    public class WalksServiceTests
    {
        private IAssetService _target;
        private Mock<ILogger<WalksService>> _logger;
        private Mock<IStorageService> _storage;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<WalksService>>();
            _storage = new Mock<IStorageService>();
            _target = new WalksService(_logger.Object, _storage.Object);
        }

        [Test]
        public async Task SaveWalkAsync_WithValidModel_SavesWalk()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            var walkModel = new WalkModel {
                Name = "Mike",
                Walk = "NiceWalk",
                Image = file.Object };

            // Act
            await _target.SaveWalkAsync(walkModel);

            // Assert
            _storage.Verify(m => m.Save(It.IsAny<Stream>(), It.IsAny<WalkModel>()), Times.Once);
        }

        [Test]
        public async Task SaveWalkAsync_WithInvalidImage_ThrowsException()
        {
            // Arrange
            var walkModel = new WalkModel
            {
                Name = "Mike",
                Walk = "NiceWalk"
            };

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _target.SaveWalkAsync(walkModel));
        }

        [Test]
        public async Task GetWalksToDisplayAsync_ReturnsListOfBlobs()
        {
            // Arrange
            var blockblob = new Mock<CloudBlockBlob>(
                MockBehavior.Loose,
                new Uri("http://bench.com/contaitner/file.ext"),
                new StorageCredentials("fakeaccoutn", Convert.ToBase64String(Encoding.Unicode.GetBytes("fakekeyval")), "fakekeyname"));
            var blobs = new List<CloudBlockBlob> { blockblob.Object };
            _storage.Setup(m => m.GetBlobs()).ReturnsAsync(blobs);

            // Act
            var result = await _target.GetWalksToDisplayAsync();

            // Assert
            var element = result.First();
            Assert.AreEqual("http://bench.com/contaitner/file.ext", element.Url.ToString());
        }
    }
}
