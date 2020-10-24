namespace Tapas.Web.ViewModels.Orders
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Tapas.Data.Models.Enums;
    using Tapas.Web.ViewModels.ShopingCartItems;

    public class OrderDetailsViewModel
    {
        public OrderDetailsViewModel()
        {
            this.Taxes = new List<SelectListItem>();
        }

        public int OrderId { get; set; }

        public string AddInfo { get; set; }

        public string UserUserName { get; set; }

        public string UserPhone { get; set; }

        public string DisplayAddress { get; set; }

        public string AddressInfo { get; set; }

        public string CreatedOn { get; set; }

        public string TimeForDelivery { get; set; }

        public decimal PackagesPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DeliveryFee { get; set; }

        public bool TakeAway { get; set; }

        public int? DeliveryTaxId { get; set; }

        public List<SelectListItem> Taxes { get; set; }

        public List<ShopingItemsViewModel> CartItems { get; set; }

        public OrderStatus Status { get; set; }

        public int CutleryCount { get; set; }

        [Display(Name = "Коментар: ")]
        public string CustomerComment { get; set; }
    }
}
