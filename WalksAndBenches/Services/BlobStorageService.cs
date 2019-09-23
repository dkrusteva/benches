using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Options;
using System.IO;
using WalksAndBenches.Models;

namespace WalksAndBenches.Services
{
    public class BlobStorageService : IStorageService
    {
        private readonly AzureStorageConfig _storageConfig;

        public BlobStorageService(IOptions<AzureStorageConfig> storageConfig)
        {
            _storageConfig = storageConfig.Value;
        }

        public async Task ConfigureBlobStorage()
        {
            var connectionString = _storageConfig.ConnectionString;
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount))
            {
                Console.WriteLine("Unable to parse connection string. Log message");
            }

            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(_storageConfig.FileContainerName);
            await blobContainer.CreateIfNotExistsAsync();


            // Set the permissions so the blobs are public.
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await blobContainer.SetPermissionsAsync(permissions);

            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string localFileName = "QuickStart_" + Guid.NewGuid().ToString() + ".txt";
            //string localFileName = "caching.png";
            //var sourceFile = Path.Combine(localPath, localFileName);
            // Write text to the file.
            //File.WriteAllText(sourceFile, "Hello, World!");

            //Console.WriteLine("Temp file = {0}", sourceFile);
            //Console.WriteLine("Uploading to Blob storage as blob '{0}'", localFileName);

            // Get a reference to the blob address, then upload the file to the blob.
            // Use the value of localFileName for the blob name.
            //CloudBlockBlob cloudBlockBlob = blobContainer.GetBlockBlobReference(localFileName);
            //await cloudBlockBlob.UploadFromFileAsync(sourceFile);
        }

        public async Task<IEnumerable<string>> GetNames()
        {
            List<string> names = new List<string>();

            var connectionString = _storageConfig.ConnectionString;
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount))
            {
                Console.WriteLine("Unable to parse conneciton string");
            }
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(_storageConfig.FileContainerName);

            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            do
            {
                resultSegment = await blobContainer.ListBlobsSegmentedAsync(continuationToken);

                names.AddRange(resultSegment.Results.OfType<ICloudBlob>().Select(b => b.Name));

                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);


            return names;
        }


        public async Task<List<CloudBlockBlob>> GetBlobs()
        {
            var blobs = new List<CloudBlockBlob>();

            var connectionString = _storageConfig.ConnectionString;
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount))
            {
                Console.WriteLine("Unable to parse conneciton string");
            }
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(_storageConfig.FileContainerName);

            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            do
            {
                resultSegment = await blobContainer.ListBlobsSegmentedAsync(continuationToken);

                blobs.AddRange(resultSegment.Results.OfType<CloudBlockBlob>());

                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);

            return blobs;
        }

        public async Task Save(Stream filestream, WalkModel model)
        {
            var connectionString = _storageConfig.ConnectionString;
            if(!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount))
            {
                Console.WriteLine("Unable to parse connection string");
            }
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(_storageConfig.FileContainerName);
            CloudBlockBlob blockblob = blobContainer.GetBlockBlobReference(model.Walk);

            blockblob.Properties.ContentType = "image/jpg";

            if (!string.IsNullOrWhiteSpace(model.Walk))
            {
                blockblob.Metadata.Add("name", model.Walk);
            }
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                blockblob.Metadata.Add("submitter", model.Name);
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                blockblob.Metadata.Add("description", model.Description);
            }

            await blockblob.UploadFromStreamAsync(filestream);
        }

    }
}
