namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Web.ViewModels.Administration.CateringFood;

    public interface ICateringFoodService
    {
        CreateModel CreateInputModel();

        Task AddCateringFoodAsync(CreateModel model);

        IEnumerable<IndexCateringFoodViewModel> GetAll();

        DetailsCateringFoodViewModel GetDetailsById(string id);

        EditCateringFoodModel GetEditModel(string id);

        Task SetEditModel(EditCateringFoodModel model);

        Task Delete(string id);

        IEnumerable<DeletedCateringProducts> GetDeletedProducts();

        Task ActivateAsync(string productId);
    }
}
