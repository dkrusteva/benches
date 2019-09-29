using WalksAndBenches.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace WalksAndBenches.Services
{
    public class WalksService : IAssetService
    {
        private readonly ILogger<WalksService> _logger;
        private readonly IStorageService _storage;

        public WalksService(ILogger<WalksService> logger, IStorageService storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public async Task SaveWalkAsync(WalkModel walk)
        {
            if (walk.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await walk.Image.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    await _storage.Save(memoryStream, walk);
                    _logger.LogInformation($"Walk: {walk.Walk}, by: {walk.Name} added.");
                }
            }
        }

        public async Task<List<WalkToDisplay>> GetWalksToDisplayAsync()
        {
            var walks = new List<WalkToDisplay>();

            var blobs = await _storage.GetBlobs();

            foreach (var blob in blobs)
            {
                await blob.FetchAttributesAsync();
                blob.Metadata.TryGetValue("description", out var d);
                blob.Metadata.TryGetValue("name", out var n);
                blob.Metadata.TryGetValue("submitter", out var s);

                var walk = new WalkToDisplay()
                {
                    Description = d,
                    Walk = n,
                    SubmittedBy = s,
                    Url = blob.Uri
                };
                walks.Add(walk);
            }

            return walks;
        }
    }
}
