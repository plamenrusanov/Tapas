namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Data.Models;
    using Tapas.Data.Models.Enums;
    using Tapas.Web.ViewModels.Orders;
    using Tapas.Web.ViewModels.Rating;

    public interface IOrdersService
    {
        OrderInpitModel GetOrderInputModel(ApplicationUser user);

        ICollection<OrdersViewModel> GetDailyOrders();

        Task<int> CreateAsync(ApplicationUser user, OrderInpitModel model);

        OrderDetailsViewModel GetDetailsById(int id);

        bool IsExists(int id);

        Task<string> ChangeStatusAsync(string status, string orderId, string setTime, string deliveryFee);

        Task<ICollection<OrderCollectionViewModel>> GetAllAsync();

        Task<ICollection<OrdersViewModel>> GetOrdersByUserNameAsync(string userName);

        Task<ICollection<UserOrderViewModel>> GetMyOrdersAsync(ApplicationUser user);

        UserOrderDetailsViewModel GetUserDetailsById(int id);

        Task SetRatingAsync(List<RatingItemDto> rating, string message);
    }
}
