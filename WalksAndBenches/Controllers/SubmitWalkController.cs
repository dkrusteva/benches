using WalksAndBenches.Models;
using WalksAndBenches.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace WalksAndBenches.Controllers
{
    public class SubmitWalkController : Controller
    {
        private readonly IAssetService walksService;
        private readonly IStorageService _storage;

        public SubmitWalkController(IAssetService service, IStorageService storage)
        {
            walksService = service;
            _storage = storage;
        }

        [HttpGet("submitWalk")]
        public IActionResult SubmitWalk()
        {
            return View();
        }

        [HttpPost("submitWalk")]
        public async Task<IActionResult> SubmitWalk(WalkModel model)
        {
            if (ModelState.IsValid)
            {
                walksService.SaveWalk(model);
                ViewBag.UserMessage = "Walk saved";
                ModelState.Clear();

                if (model.Image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.Image.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        await _storage.Save(memoryStream, model);
                    }
                }
            }

            return View();
        }
    }
}
