using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WalksAndBenches.IntegrationTests;
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
            _containerName = Guid.NewGuid().ToString().ToLower();

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

        private CloudBlobContainer GetContainerReference(string containerName)
        {
            return CloudStorageAccount.Parse(_connectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(containerName);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}