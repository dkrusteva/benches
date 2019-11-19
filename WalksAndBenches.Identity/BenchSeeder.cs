using WalksAndBenches.Identity.Entities;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WalksAndBenches.Identity
{
    public class BenchSeeder
    {
        private readonly BenchContext _ctx;
        private readonly IHostingEnvironment _hosting;
        private readonly UserManager<BenchUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BenchSeeder(
            BenchContext ctx,
            IHostingEnvironment hosting,
            UserManager<BenchUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _ctx = ctx;
            _hosting = hosting;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            _ctx.Database.Migrate();
            var benchesJsonPath = "D:/home/site/wwwroot";

            var adminRole = Constants.BenchAdministratorsRole;
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(adminRole));
            }
            var regUserRole = Constants.BenchRegisteredUsersRole;
            if (!await _roleManager.RoleExistsAsync(regUserRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(regUserRole));
            }

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

            var superuser = await _userManager.FindByEmailAsync("buzzLightyear@gmail.com");
            if (superuser == null)
            {
                superuser = new BenchUser()
                {
                    UserName = "buzzLightyear@gmail.com",
                    FirstName = "Buzz",
                    LastName = "Lightyear",
                    Email = "buzzLightyear@gmail.com"
                };

                var result = await _userManager.CreateAsync(superuser, "T0y$tory");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create BenchUser.");
                }
            }

            user = await _userManager.FindByEmailAsync("joebloggs@gmail.com");
            await _userManager.AddToRoleAsync(user, adminRole);
            superuser = await _userManager.FindByEmailAsync("buzzLightyear@gmail.com");
            await _userManager.AddToRoleAsync(superuser, regUserRole);

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
