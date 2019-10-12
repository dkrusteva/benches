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
using Microsoft.AspNetCore.Identity;

namespace WalksAndBenches.Data
{
    public class BenchSeeder
    {
        private readonly BenchContext _ctx;
        private readonly IHostingEnvironment _hosting;
        private readonly UserManager<BenchUser> _userManager;

        public BenchSeeder(BenchContext ctx, IHostingEnvironment hosting, UserManager<BenchUser> userManager)
        {
            _ctx = ctx;
            _hosting = hosting;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            _ctx.Database.Migrate();
            var benchesJsonPath = "D:/home/site/wwwroot";

            var user = await _userManager.FindByEmailAsync("joebloggs@gmail.com");
            if(user == null)
            {
                user = new BenchUser()
                {
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = "joebloggs@gmail.com",
                    UserName = "joebloggs@gmail.com"
                };

                var result = await _userManager.CreateAsync(user, "P@ssw0rd");
                if(result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create BenchUser.");
                }
            }

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
