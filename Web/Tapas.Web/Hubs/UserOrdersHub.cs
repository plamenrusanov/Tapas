namespace Tapas.Web.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Rating;

    public class UserOrdersHub : Hub
    {
        private readonly IOrdersService ordersService;
        private readonly ILogger<UserOrdersHub> logger;

        public UserOrdersHub(
            IOrdersService ordersService,
            ILogger<UserOrdersHub> logger)
        {
            this.ordersService = ordersService;
            this.logger = logger;
        }

        public async Task UserSetRating(List<RatingItemDto> ratings, string message)
        {
            try
            {
                await this.ordersService.SetRatingAsync(ratings, message);
                await this.Clients.Caller.SendAsync("SuccessfullySetRating");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
            }
        }
    }
}
