using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using System.Data;

namespace ProiectLicenta.Seed
{
    public class SeedDatabase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public SeedDatabase(RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            this._roleManager = roleManager;
            this._dataContext = dataContext;
        }
        public async Task SeedDb()
        {
            if (_dataContext.Roles.Any())
            {
                return;
            }

            string[] roles =
            {
                "Artist",
                "Client"
            };

            foreach (var role in roles)
            {
                var result = await _roleManager.RoleExistsAsync(role);
                if (!result)
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role
                    });
                }
            }
            await _dataContext.SaveChangesAsync();
        }
    }
}
