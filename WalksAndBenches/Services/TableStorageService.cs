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


namespace WalksAndBenches.Services
{
    public class TableStorageService : ITableStorageService
    {

        private const string connectionString = "DefaultEndpointsProtocol=http;AccountName=localhost;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;TableEndpoint=http://localhost:8902/;";

        public async Task SaveBench(WalkToSave entityToSave)
        {
            await ContactTableStorage();
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

            var retrieveOperation = TableOperation.Retrieve<WalkToSave>("James", "Bench");
            var result = await table.ExecuteAsync(retrieveOperation);
            var bench = result.Result as WalkToSave;


            Console.WriteLine("| {0, 10} | {1, 30} | {2, 10} | {3, 10} |", "Author", "Entry name", "Location", "Url");
            Console.WriteLine("| {0, 10} | {1, 30} | {2, 10} | {3, 10} |", bench.SubmitterName, bench.WalkName, bench.Location, bench.Url);
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
            var lens = new WalkToSave(sub, name)
            {
                Description = descr,
                Location = loc,
                SubmitterName = sub,
                WalkName = name,
                Url = url
            };
            // Add the entity
            TableOperation insertOrMerge = TableOperation.InsertOrMerge(lens);
            TableResult result = await table.ExecuteAsync(insertOrMerge);
        }
    }
}
