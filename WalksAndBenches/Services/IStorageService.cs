using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WalksAndBenches.Models;

namespace WalksAndBenches.Services
{
    public interface IStorageService
    {
        Task ConfigureBlobStorage();

        Task<Uri> Save(Stream filestream, WalkModel model);

        Task<List<CloudBlockBlob>> GetBlobs();
    }
}
