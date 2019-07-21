using WalksAndBenches.Data.Entities;
using WalksAndBenches.Services;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace WalksAndBenches.Data
{
    public class BenchSeeder
    {
        private readonly BenchContext _ctx;
        private readonly IHostingEnvironment _hosting;
        private readonly IStorageService _storage;

        public BenchSeeder(BenchContext ctx, IHostingEnvironment hosting, IStorageService storage)
        {
            _ctx = ctx;
            _hosting = hosting;
            _storage = storage;
        }

        public void Seed()
        {
            _ctx.Database.Migrate();
            var benchesJsonPath = "D:/home/site/wwwroot";

            if (!_ctx.Walks.Any())
            {
                var filepath = Path.Combine(_hosting.ContentRootPath, "Data/benchwalk.json");
                var json = File.ReadAllText(filepath);
                var walks = JsonConvert.DeserializeObject<IEnumerable<Walks>>(json);

                _ctx.Walks.AddRange(walks);
                _ctx.SaveChanges();
            }
        }
    }
}
