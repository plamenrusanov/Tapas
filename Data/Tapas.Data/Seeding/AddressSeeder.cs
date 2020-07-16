namespace Tapas.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Tapas.Common;
    using Tapas.Data.Models;

    public class AddressSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var user = dbContext.Users.FirstOrDefault();
            if (!dbContext.DeliveryAddresses.Any(a => a.DisplayName == GlobalConstants.TakeAway))
            {
                await dbContext.DeliveryAddresses.AddAsync(new DeliveryAddress()
                {
                    DisplayName = GlobalConstants.TakeAway,
                    ApplicationUserId = user.Id,
                });
            }
        }
    }
}
