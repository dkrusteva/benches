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
        private const string walkName = "walk with a bench";
        private const string description = "A nice bench!";
        private const string storageUrl = "http://bench.com/contaitner/file.ext";

        private IAssetService _target;
        private Mock<ILogger<WalksService>> _logger;
        private Mock<IStorageService> _storage;
        private Mock<ITableStorageService> _tableStorage;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<WalksService>>();
            _storage = new Mock<IStorageService>();
            _tableStorage = new Mock<ITableStorageService>();
            _target = new WalksService(_logger.Object, _storage.Object, _tableStorage.Object);
        }

        [Test]
        public async Task SaveWalkAsync_WithValidModel_SavesWalk()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            var walkModel = new WalkModel {
                SubmittedBy = submittedBy,
                WalkName = walkName,
                UploadedImage = file.Object };
            _storage.Setup(m => m.Save(It.IsAny<Stream>(), It.IsAny<WalkModel>())).ReturnsAsync(new Uri(storageUrl));

            // Act
            await _target.SaveWalkAsync(walkModel);

            // Assert
            _storage.Verify(m => m.Save(It.IsAny<Stream>(), It.IsAny<WalkModel>()), Times.Once);
            _tableStorage.Verify(m => m.SaveBench(It.IsAny<WalkToSave>()), Times.Once);
        }

        [Test]
        public async Task SaveWalkAsync_WithInvalidUploadedImage_ThrowsException()
        {
            // Arrange
            var walkModel = new WalkModel
            {
                SubmittedBy = submittedBy,
                WalkName = walkName
            };

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _target.SaveWalkAsync(walkModel));
        }

        [Test]
        public async Task GetWalksToDisplayAsync_ReturnsListOfBlobs()
        {
            // Arrange
            var sampleEntity = new WalkToSave(submittedBy, walkName)
            {
                Description = description,
                Location = "nowhere",
                Url = storageUrl
            };
            _tableStorage.Setup(m => m.GetAllEntities()).ReturnsAsync(new List<WalkToSave> { sampleEntity });

            // Act
            var result = await _target.GetWalksToDisplayAsync();

            // Assert
            var element = result.First();
            Assert.AreEqual(storageUrl, element.StorageUrl.ToString());
            Assert.AreEqual(submittedBy, element.SubmittedBy);
            Assert.AreEqual(walkName, element.WalkName);
            Assert.AreEqual(description, element.Description);
        }
    }
}
