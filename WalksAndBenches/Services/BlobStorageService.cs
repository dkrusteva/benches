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
using Microsoft.WindowsAzure.Storage.Table;
using WalksAndBenches.Identity.Entities;
using CloudTable = Microsoft.WindowsAzure.Storage.Table.CloudTable;
using CloudTableClient = Microsoft.WindowsAzure.Storage.Table.CloudTableClient;

namespace WalksAndBenches.Services
{
    public class BlobStorageService : IStorageService
    {
        private readonly AzureStorageConfig _storageConfig;
        private CloudBlobContainer _blobContainer;
        private const string connectionString = "DefaultEndpointsProtocol=http;AccountName=localhost;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;TableEndpoint=http://localhost:8902/;";

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

            await ContactTableStorage();

            return blobs;
        }

        public async Task Save(Stream filestream, WalkModel model)
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
        }



        private async Task ContactTableStorage()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("testtable");
            await table.CreateIfNotExistsAsync();

            Console.WriteLine("Table created. Populating...");
            //await table.ExecuteAsync(TableOperation.Insert(new DynamicTableEntity("partitionKey", "rowKey")));

            await InsertEntity(table, "A nice bench", "not far", "James", "Bench", "bb");
            await InsertEntity(table, "Bench bench", "Bristol", "James", "Bench2", "bb2");
            await InsertEntity(table, "It is a wooden bench", "Bristol bench shop", "Caren", "Bench3", "bb3");

            Console.WriteLine("Tables created and populated.");

            await ReadTable(table);

        }

        private async Task ReadTable(CloudTable table)
        {
            // Read the table and display it here.
            Console.WriteLine("Reading the contents of the Lenses table...");

            var retrieveOperation = TableOperation.Retrieve<Bench>("James", "Bench");
            var result = await table.ExecuteAsync(retrieveOperation);
            var bench = result.Result as Bench;


            Console.WriteLine("| {0, 10} | {1, 30} | {2, 10} | {3, 10} |", "Author", "Entry name", "Location", "Url"); 
            Console.WriteLine("| {0, 10} | {1, 30} | {2, 10} | {3, 10} |", bench.SubmitterName, bench.BenchName, bench.Location, bench.Url);
            Console.WriteLine("Finished reading the contents of the Lenses table...");
        }

        private async Task InsertEntity(CloudTable table,
            string descr,
            string loc,
            string sub,
            string name,
            string url)
        {
            // Create an entity and set properties
            var lens = new Bench(sub, name)
            {
                Description = descr,
                Location = loc,
                SubmitterName = sub,
                BenchName = name,
                Url = url
            };
            // Add the entity
            TableOperation insertOrMerge = TableOperation.InsertOrMerge(lens);
            TableResult result = await table.ExecuteAsync(insertOrMerge);
        }
    }


    public class Bench : TableEntity
    {
        public Bench()
        {

        }

        public Bench(string submitterName, string benchName)
        {
            PartitionKey = submitterName;
            SubmitterName = submitterName;
            RowKey = benchName;
            BenchName = benchName;
        }

        public string Description { get; set; }
        public string Location { get; set; }
        public string SubmitterName { get; set; }
        public string BenchName { get; set; }
        public string Url { get; set; }

    }
}
