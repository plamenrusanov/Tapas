namespace Tapas.Web.ViewModels.ShopingCart
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Tapas.Web.ViewModels.ShopingCartItems;

    public class ShopingCartViewModel
    {
        public ShopingCartViewModel()
        {
            this.ShopingItems = new List<ShopingItemsViewModel>();
            this.Cutlery = this.CreateCutleryList();
        }

        public string Id { get; set; }

        public string UserId { get; set; }

        public decimal PackegesPrice { get; set; }

        public bool TakeAway { get; set; }

        public decimal TotalPrice => this.ShopingItems.Sum(x => x.ItemPrice)
                                   + this.PackegesPrice;

        public List<ShopingItemsViewModel> ShopingItems { get; set; }

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
