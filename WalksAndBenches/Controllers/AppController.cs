using WalksAndBenches.Data;
using WalksAndBenches.Models;
using WalksAndBenches.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WalksAndBenches.Controllers
{
    public class AppController : Controller
    {
        private readonly IAssetService walksService;
        //private readonly BenchContext _context;
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storage;

        public AppController(IAssetService service, IConfiguration configuration, IStorageService storage)
        {
            walksService = service;
            //_context = context;
            _configuration = configuration;
            _storage = storage;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("submitWalk")]
        public IActionResult SubmitWalk()
        {
            return View();
        }
        
        [HttpPost("submitWalk")]
        public async Task<IActionResult> SubmitWalk(WalkModel model)
        {
            if(ModelState.IsValid)
            {
                walksService.SaveWalk(model);
                ViewBag.UserMessage = "Walk saved";
                ModelState.Clear();

                if(model.Image != null)
                {
                    using(var memoryStream = new MemoryStream())
                    {
                        await model.Image.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        await _storage.Save(memoryStream, model);
                    }
                }
            }

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Explore()
        {
            var walks = new List<WalkToDisplay>();

            var blobs = await _storage.GetBlobs();
            var urls = new List<Uri>();
            foreach(var blob in blobs)
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

            //var results = _context.Walks.ToList();
            return View(walks);
        }
    }
}
