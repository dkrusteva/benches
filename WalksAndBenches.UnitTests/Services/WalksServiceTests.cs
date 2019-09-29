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
        private const string submittedBy = "Mike";
        private const string walk = "walk with a bench";
        private const string description = "A nice bench!";
        private const string url = "http://bench.com/contaitner/file.ext";

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
                Name = submittedBy,
                Walk = walk,
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
                Name = submittedBy,
                Walk = walk
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
                new Uri(url),
                new StorageCredentials("fakeaccoutn", Convert.ToBase64String(Encoding.Unicode.GetBytes("fakekeyval")), "fakekeyname"));
            blockblob.Object.Metadata.Add("name", walk);
            blockblob.Object.Metadata.Add("description", description);
            blockblob.Object.Metadata.Add("submitter", submittedBy);
            _storage.Setup(m => m.GetBlobs()).ReturnsAsync(new List<CloudBlockBlob> { blockblob.Object });

            // Act
            var result = await _target.GetWalksToDisplayAsync();

            // Assert
            var element = result.First();
            Assert.AreEqual(url, element.Url.ToString());
            Assert.AreEqual(submittedBy, element.SubmittedBy);
            Assert.AreEqual(walk, element.Walk);
            Assert.AreEqual(description, element.Description);
        }
    }
}
