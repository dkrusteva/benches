using WalksAndBenches.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WalksAndBenches.Services
{
    public class WalksService : IAssetService
    {
        private readonly ILogger<WalksService> _logger;
        private readonly IStorageService _storage;
        private readonly ITableStorageService _tableStorage;

        public WalksService(ILogger<WalksService> logger, IStorageService storage, ITableStorageService tableStorage)
        {
            _logger = logger;
            _storage = storage;
            _tableStorage = tableStorage;
        }

        public async Task SaveWalkAsync(WalkModel walk)
        {
            if (walk.UploadedImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await walk.UploadedImage.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    var uri = await _storage.Save(memoryStream, walk);
                    _logger.LogInformation($"Walk: {walk.WalkName}, by: {walk.SubmittedBy} added.");
                    var entity = MapWalkModelToWalkToSave(walk, uri);
                    await _tableStorage.SaveBench(entity);
                }
            }
            else
            {
                throw new ArgumentNullException("Image", "The supplied model does not have an image");
            }
        }

        public async Task<List<WalkToDisplay>> GetWalksToDisplayAsync()
        {
            var entities = await _tableStorage.GetAllEntities();
            var entitiesToDisplay = entities.Select(e => MapWalkToSaveToWalkToDisplay(e)).ToList();
            return entitiesToDisplay;
        }

        private async Task<WalkToDisplay> MapBlobToWalkToDisplay(CloudBlockBlob blob)
        {
            await blob.FetchAttributesAsync();
            blob.Metadata.TryGetValue("description", out var d);
            blob.Metadata.TryGetValue("walkname", out var n);
            blob.Metadata.TryGetValue("submittedby", out var s);

            var walk = new WalkToDisplay()
            {
                Description = d,
                WalkName = n,
                SubmittedBy = s,
                StorageUrl = blob.Uri
            };
            return walk;
        }

        private  WalkToSave MapWalkModelToWalkToSave(WalkModel walk, Uri uri)
        {
            return new WalkToSave(walk.SubmittedBy, walk.WalkName)
            {
                Description = walk.Description,
                Location = "Local",
                SubmitterName = walk.SubmittedBy,
                WalkName = walk.WalkName,
                Url = uri.ToString()
            };
        }

        private WalkToDisplay MapWalkToSaveToWalkToDisplay(WalkToSave walk)
        {
            return new WalkToDisplay
            {
                Description = walk.Description,
                SubmittedBy = walk.SubmitterName,
                WalkName = walk.WalkName,
                StorageUrl = new Uri(walk.Url)
            };
        }
    }
}
