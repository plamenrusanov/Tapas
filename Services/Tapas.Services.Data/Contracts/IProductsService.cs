﻿namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Web.ViewModels.Administration.Products;

    public interface IProductsService
    {
        Task AddAsync(ProductInputViewModel inputModel);

        MenuProductViewModel GetProductById(string productId);

        DetailsProductViewModel GetDetailsProductById(string productId);

        bool ExistProductById(string productId);

        EditProductModel GetEditProductById(string productId);

        Task EditProductAsync(EditProductModel model);

        DeleteProductViewModel GetDeleteProductById(string productId);

        Task DeleteProductAsync(string productId);

        ICollection<DetailsProductViewModel> GetAllProducts(bool isDeleted);

        void Activate(string productId);

        ProductsIndexViewModel CategoryWhitProducts(string categoryId = null);
    }
}
