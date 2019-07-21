using WalksAndBenches.Models;
using WalksAndBenches.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace WalksAndBenches.Controllers
{
    public class ExploreController : Controller
    {
        private readonly IStorageService _storage;

        public ExploreController(IStorageService storage)
        {
            _storage = storage;
        }

        public async Task<IActionResult> Explore()
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

            return View(walks);
        }
    }
}
