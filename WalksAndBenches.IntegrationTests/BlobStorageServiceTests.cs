using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NUnit.Framework;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WalksAndBenches.IntegrationTests;
using WalksAndBenches.Models;
using WalksAndBenches.Services;

namespace WalksAndBenches.IntegrationTests
{
    [TestFixture]
    public class BlobStorageServiceTests
    {
        private WebApplicationFactory<WalksAndBenches.Startup> _factory;
        private HttpClient _client;
        private string _containerName;
        private const string _connectionString = "UseDevelopmentStorage=true";
        private IStorageService _sut;

        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new BenchesWebApplicationFactory();
            _client = _factory.CreateClient();
            _containerName = $"Container_{Guid.NewGuid().ToString().ToLower()}";

            AzureStorageConfig azureAppSettings = new AzureStorageConfig()
            {
                ConnectionString = _connectionString,
                FileContainerName = _containerName
            };
            IOptions<AzureStorageConfig> options = Options.Create(azureAppSettings);
            _sut = new BlobStorageService(options);
        }

        [Test]
        public async Task AboutPage_GetsReturned()
        {
            var url = "/App/About";

            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task ConfigureBlobStorage_WithContainerName_ConfiguresContainer()
        {
            // Act
            await _sut.ConfigureBlobStorage();

            // Assert
            var container = GetContainerReference(_containerName);
            Assert.IsTrue(await container.ExistsAsync());
            var permissions = await container.GetPermissionsAsync();
            Assert.IsTrue(permissions.PublicAccess.Equals(BlobContainerPublicAccessType.Blob));
        }

        [Test]
        public async Task GetBlobs_WithContainerName_GetsBlobsFromContainer()
        {
            // Arrange
            await _sut.ConfigureBlobStorage();
            var model = new WalkModel()
            {
                WalkName = "Test walk",
                Description = "A walk in the test realm",
                SubmittedBy = "Test user"
            };
            var otherModel = new WalkModel()
            {
                WalkName = "Other test walk",
                Description = "A walk in the other test realm",
                SubmittedBy = "Other test user"
            };
            using (var stream = GenerateStreamFromString("a,b \n c,d"))
            {
                await _sut.Save(stream, model);
            }
            using (var stream = GenerateStreamFromString("e,f \n g,h"))
            {
                await _sut.Save(stream, otherModel);
            }

            // Act
            var blobs = await _sut.GetBlobs();

            // Assert
            Assert.AreEqual(2, blobs.Count);
            var text = await blobs[0].DownloadTextAsync();
            Assert.AreEqual("e,f \n g,h", text);
            var otherText = await blobs[1].DownloadTextAsync();
            Assert.AreEqual("a,b \n c,d", otherText);
            await AssertMetadata(blobs[1], "Other test walk", "A walk in the other test realm", "Other test user");
            await AssertMetadata(blobs[1], "Test walk", "A walk in the test realm", "Test user");
        }

        [Test]
        public async Task Save_WithFileAndModel_SavesBlobToContainer()
        {
            // Arrange
            await _sut.ConfigureBlobStorage();
            var model = new WalkModel()
            {
                WalkName = "Test walk",
                Description = "A walk in the test realm",
                SubmittedBy = "Test user"
            };

            // Act
            using (var stream = GenerateStreamFromString("a,b \n c,d"))
            {
                await _sut.Save(stream, model);
            }

            // Assert
            var blob = GetContainerReference(_containerName)
                .GetBlockBlobReference("Test walk");
            Assert.IsTrue(await blob.ExistsAsync());
            var text = await blob.DownloadTextAsync();
            Assert.AreEqual("a,b \n c,d", text);
            await AssertMetadata(blob, "Test walk", "A walk in the test realm", "Test user");
        }

        private async Task AssertMetadata(CloudBlockBlob blob, string name, string description, string submittedBy)
        {
            await blob.FetchAttributesAsync();
            blob.Metadata.TryGetValue("description", out var d);
            blob.Metadata.TryGetValue("walkname", out var n);
            blob.Metadata.TryGetValue("submittedby", out var s);

            Assert.AreEqual("Test walk", n);
            Assert.AreEqual("A walk in the test realm", d);
            Assert.AreEqual("Test user", s);
        }

        private CloudBlobContainer GetContainerReference(string containerName)
        {
            return CloudStorageAccount.Parse(_connectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(containerName);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(s);
            writer.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        [TearDown]
        public async Task TearDown()
        {
            var container = GetContainerReference(_containerName);
            await container.DeleteIfExistsAsync();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}