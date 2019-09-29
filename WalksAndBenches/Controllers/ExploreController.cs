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
        private readonly IAssetService _assets;

        public ExploreController(IAssetService assets)
        {
            _assets = assets;
        }

        public async Task<IActionResult> Explore()
        {
            var walks = await _assets.GetWalksToDisplayAsync();

            return View(walks);
        }
    }
}
