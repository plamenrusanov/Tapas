namespace Tapas.Web.ViewModels.ShopingCartItems
{
    using System.Collections.Generic;
    using System.Linq;

    using Tapas.Web.ViewModels.Administration.Sizes;
    using Tapas.Web.ViewModels.Extras;

    public class ShopingItemsViewModel
    {
        public ShopingItemsViewModel()
        {
            this.Extras = new List<ExtraCartItemModel>();
        }

        public int Id { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }

        public decimal ItemPrice => (this.ProductPrice * this.Quantity) + this.Extras.Sum(x => x.Price * x.Quantity);

        public string Description { get; set; }

        public int SizeId { get; set; }

        public byte Rating { get; set; }

        public ProductSizeViewModel Size { get; set; }

        public List<ExtraCartItemModel> Extras { get; set; }
    }
}
