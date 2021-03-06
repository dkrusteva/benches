﻿using Microsoft.Extensions.Configuration;
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
        private CloudBlobContainer _blobContainer;
        
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
                throw new ArgumentException("Unable to parse connection string. Log message");
                // Question - at this point the development exceptions page is not available yet
                // so message will not be displayed in a useful way. Is is still needed here?
                // Furthermore, the Async call below can fail if the container is not up as well
            }

            var blobClient = storageAccount.CreateCloudBlobClient();
            _blobContainer = blobClient.GetContainerReference(_storageConfig.FileContainerName);
            await _blobContainer.CreateIfNotExistsAsync();


            // Set the permissions so the blobs are public.
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await _blobContainer.SetPermissionsAsync(permissions);
        }

        public async Task<List<CloudBlockBlob>> GetBlobs()
        {
            var blobs = new List<CloudBlockBlob>();

            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            do
            {
                resultSegment = await _blobContainer.ListBlobsSegmentedAsync(continuationToken);

                blobs.AddRange(resultSegment.Results.OfType<CloudBlockBlob>());

                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);

            return blobs;
        }

        public async Task<Uri> Save(Stream filestream, WalkModel model)
        {
            CloudBlockBlob blockblob = _blobContainer.GetBlockBlobReference(model.WalkName);

            blockblob.Properties.ContentType = "image/jpg";

            if (!string.IsNullOrWhiteSpace(model.WalkName))
            {
                blockblob.Metadata.Add("walkname", model.WalkName);
            }
            if (!string.IsNullOrWhiteSpace(model.SubmittedBy))
            {
                blockblob.Metadata.Add("submittedby", model.SubmittedBy);
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                blockblob.Metadata.Add("description", model.Description);
            }

            await blockblob.UploadFromStreamAsync(filestream);

            return _blobContainer.GetBlockBlobReference(model.WalkName).Uri;
        }
    }
}
