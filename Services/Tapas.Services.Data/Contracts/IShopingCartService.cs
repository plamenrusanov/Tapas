namespace Tapas.Services.Data.Contracts
{
    using Tapas.Web.ViewModels.ShopingCart;

    public interface IShopingCartService
    {
        AddItemViewModel GetShopingModel(string productId, int? sizeId = null);
    }
}
