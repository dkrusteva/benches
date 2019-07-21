using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalksAndBenches.Services
{
    public class BlobStorage : IStorage
    {
        private readonly ILogger<BlobStorage> logger;

        public BlobStorage(ILogger<BlobStorage> logger)
        {
            this.logger = logger;
        }
    }
}
