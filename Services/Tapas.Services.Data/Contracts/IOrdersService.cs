namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Data.Models;
    using Tapas.Web.ViewModels.Orders;
    using Tapas.Web.ViewModels.Rating;

    public interface IOrdersService
    {
        IEnumerable<OrdersViewModel> GetDailyOrders();

        Task<int> CreateAsync(ApplicationUser user, OrderInpitModel model);

        OrderDetailsViewModel GetDetailsById(int id);

        bool IsExists(int id);

        Task<string> ChangeStatusAsync(string status, string orderId, string setTime, string deliveryFee);

        Task<IEnumerable<OrderCollectionViewModel>> GetAllAsync(int skip, int take);

        Task<IEnumerable<OrdersViewModel>> GetOrdersByUserNameAsync(string userName);

        Task<IEnumerable<UserOrderViewModel>> GetMyOrdersAsync(ApplicationUser user);

        UserOrderDetailsViewModel GetUserDetailsById(int id);

        Task SetRatingAsync(IEnumerable<RatingItemDto> rating, string message);
    }
}
