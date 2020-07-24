namespace Tapas.Web.ViewModels.Orders
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Tapas.Data.Models.Enums;
    using Tapas.Web.ViewModels.ShopingCartItems;

    public class UserOrderDetailsViewModel
    {
        public int OrderId { get; set; }

        public string CreatedOn { get; set; }

        public string TimeForDelivery { get; set; }

        public decimal PackagesPrice { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal TotalPrice { get; set; }

        public bool TakeAway { get; set; }

        public List<ShopingItemsViewModel> CartItems { get; set; }

        public OrderStatus Status { get; set; }

        public int CutleryCount { get; set; }

        [Display(Name ="Коментар: ")]
        public string CustomerComment { get; set; }
    }
}
