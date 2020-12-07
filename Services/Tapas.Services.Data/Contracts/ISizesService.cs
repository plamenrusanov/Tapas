namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;

    using Tapas.Web.ViewModels.Administration.Sizes;

    public interface ISizesService
    {
        IEnumerable<EditSizeModel> GetSizesOfProduct(string productId);

        bool ExistById(int sizeId);

        ProductSizeViewModel GetDetailModel(int sizeId);

        ExtraSize GetExtraSize(int index);

        string Remove(int id);
    }
}
