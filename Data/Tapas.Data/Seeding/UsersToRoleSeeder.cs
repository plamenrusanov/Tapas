namespace Tapas.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Tapas.Common;
    using Tapas.Data.Models;

    internal class UsersToRoleSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await this.AddUserToRole(
                GlobalConstants.AdministratorName,
                GlobalConstants.AdministratorName,
                userManager);
            await this.AddUserToRole(
                GlobalConstants.AdministratorName,
                GlobalConstants.OperatorName,
                userManager);
            await this.AddUserToRole(
                GlobalConstants.OperatorName,
                GlobalConstants.OperatorName,
                userManager);
        }

        private async Task AddUserToRole(
            string userName,
            string role,
            UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (await userManager.IsInRoleAsync(user, role))
            {
                return;
            }

            await userManager.AddToRoleAsync(user, role);
        }
    }
}
