namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Web.ViewModels.Administration.Categories;

    public interface ICategoriesService
    {
        string GetCategoryNameById(string categoryId);

        bool ExistCategoryByName(string categoryName);

        bool ExistCategoryById(string categoryId);

        Task AddAsync(string categoryName);

        IEnumerable<CategoryViewModel> All();

        CategoryViewModel GetCategoryViewModelById(string categoryId);

        void Edit(CategoryViewModel categoryViewModel);

        void Remove(string categoryId);

        Task SavePositions(IEnumerable<CategoryViewModel> models);
    }
}
