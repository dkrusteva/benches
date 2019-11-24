using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using WalksAndBenches.Models;
using Microsoft.WindowsAzure.Storage.Table;
using WalksAndBenches.Identity.Entities;
using CloudTable = Microsoft.WindowsAzure.Storage.Table.CloudTable;
using CloudTableClient = Microsoft.WindowsAzure.Storage.Table.CloudTableClient;
using System.Net;
using Microsoft.Extensions.Options;

namespace WalksAndBenches.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly AzureStorageConfig _storageConfig;

        public TableStorageService(IOptions<AzureStorageConfig> storageConfig)
        {
            _storageConfig = storageConfig.Value;
        }
        private async Task ContactTableStorage()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_storageConfig.TableConnectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(_storageConfig.MetadataTableName);
            await table.CreateIfNotExistsAsync();
        }

        public async Task SaveBench(WalkToSave entityToSave)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_storageConfig.TableConnectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(_storageConfig.MetadataTableName);
            await table.CreateIfNotExistsAsync();

            var result = await InsertEntity(table, entityToSave);

            if(result.HttpStatusCode != (int)HttpStatusCode.NoContent)
            {
                throw new ApplicationException("Failed to insert entity in database.");
            }
        }

        public async Task<List<WalkToSave>> GetAllEntities()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_storageConfig.TableConnectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(_storageConfig.MetadataTableName);
            await table.CreateIfNotExistsAsync();
            var query = new TableQuery<WalkToSave>();

            var entities = new List<WalkToSave>();
            TableContinuationToken token = null;
            TableQuerySegment<WalkToSave> resultSegment = null;

            do
            {
                resultSegment = await table.ExecuteQuerySegmentedAsync<WalkToSave>(query, token);
                entities.AddRange(resultSegment.Results.OfType<WalkToSave>());
                token = resultSegment.ContinuationToken;
            }
            while (token != null);

            return entities;
        }

        private async Task<TableResult> InsertEntity(CloudTable table, WalkToSave entityToSave)
        {
            // Create an entity and set properties
            var lens = new WalkToSave(entityToSave.SubmitterName, entityToSave.WalkName)
            {
                Description = entityToSave.Description,
                Location = entityToSave.Location,
                SubmitterName = entityToSave.SubmitterName,
                WalkName = entityToSave.WalkName,
                Url = entityToSave.Url
            };
            // Add the entity
            TableOperation insertOrMerge = TableOperation.InsertOrMerge(lens);
            return await table.ExecuteAsync(insertOrMerge);
        }
    }
}
