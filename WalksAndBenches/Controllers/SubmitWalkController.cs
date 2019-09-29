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
        private readonly IAssetService _assets;

        public SubmitWalkController(IAssetService assets)
        {
            _assets = assets;
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
                await _assets.SaveWalkAsync(model);
                ViewBag.UserMessage = "Walk saved";
                ModelState.Clear();
            }

            return View();
        }
    }
}
