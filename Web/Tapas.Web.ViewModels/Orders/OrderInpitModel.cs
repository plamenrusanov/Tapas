namespace Tapas.Web.ViewModels.Orders
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Tapas.Web.ViewModels.Addreses;
    using Tapas.Web.ViewModels.ShopingCartItems;

    public class OrderInpitModel
    {
        public OrderInpitModel()
        {
            this.Addresses = new List<AddressViewModel>();
            this.Cutlery = this.CreateCutleryList();
        }

        public string AddInfo { get; set; }

        public string ApplicationUserId { get; set; }

        [Required(ErrorMessage = "Полето Адрес е задължително!")]
        public string AddressId { get; set; }

        public decimal PackegesPrice { get; set; }

        public decimal DeliveryFee { get; set; }

        public bool TakeAway { get; set; }

        public decimal OrderPrice { get; set; }

        public DateTime DelayedDelivery { get; set; }

        public List<AddressViewModel> Addresses { get; set; }

        public List<ShopingItemsViewModel> CartItems { get; set; }

        public int CutleryCount { get; set; }

        public List<SelectListItem> Cutlery { get; set; }

        private List<SelectListItem> CreateCutleryList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Value = "0", Selected = true,  Text = "Не желая прибори" },
                new SelectListItem() { Value = "1", Selected = false,  Text = "1 комплект прибори" },
                new SelectListItem() { Value = "2", Selected = false,  Text = "2 комплекта прибори" },
                new SelectListItem() { Value = "3", Selected = false,  Text = "3 комплекта прибори" },
                new SelectListItem() { Value = "4", Selected = false,  Text = "4 комплекта прибори" },
                new SelectListItem() { Value = "5", Selected = false,  Text = "5 комплекта прибори" },
            };
        }
    }
}
