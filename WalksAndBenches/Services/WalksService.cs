using WalksAndBenches.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalksAndBenches.Services
{
    public class WalksService : IAssetService
    {
        private readonly ILogger<WalksService> _logger;

        public WalksService(ILogger<WalksService> logger)
        {
            _logger = logger;
        }

        public void SaveWalk(WalkModel walk)
        {
            _logger.LogInformation($"Walk: {walk.Walk}, by: {walk.Name} added.");
        }
    }
}
