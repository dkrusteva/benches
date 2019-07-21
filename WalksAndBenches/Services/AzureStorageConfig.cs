using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalksAndBenches.Services
{
    public class AzureStorageConfig
    {
        public string ConnectionString { get; set; }
        public string FileContainerName { get; set; }
    }
}
