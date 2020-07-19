namespace Tapas.Web.Hubs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;
    using Tapas.Web.ViewModels.Rating;

    public class UserOrdersHub : Hub
    {
        public async Task UserSetRating(List<RatingItemDto> ratings, string message)
        {
            foreach (var item in ratings)
            {
                System.Console.WriteLine($"{item.ItemId} - {item.Rating}");
            }

            System.Console.WriteLine(message);
        }
    }
}
