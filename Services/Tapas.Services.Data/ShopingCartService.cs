namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Tapas.Common;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Allergens;
    using Tapas.Web.ViewModels.Administration.Products;
    using Tapas.Web.ViewModels.Administration.Sizes;
    using Tapas.Web.ViewModels.ShopingCart;

    public class ShopingCartService : IShopingCartService
    {
        private readonly IDeletableEntityRepository<MenuProduct> productRepository;
        private readonly IDeletableEntityRepository<ProductSize> sizeRepository;
        private readonly IExtrasService extrasService;

        public ShopingCartService(
            IDeletableEntityRepository<MenuProduct> productRepository,
            IDeletableEntityRepository<ProductSize> sizeRepository,
            IExtrasService extrasService)
        {
            this.productRepository = productRepository;
            this.sizeRepository = sizeRepository;
            this.extrasService = extrasService;
        }

        public AddItemViewModel GetShopingModel(string productId, int? sizeId = null)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException();
            }

            var product = this.productRepository
                    .All()
                    .Where(x => x.Id == productId)
                    .FirstOrDefault();
            if (product is null)
            {
                throw new Exception();
            }

            ProductSize size = null;
            if (sizeId.HasValue)
            {
                size = this.sizeRepository.All()
                    .Where(x => x.Id == sizeId)
                    .FirstOrDefault();
            }

            var model = new AddItemViewModel()
            {
                Product = new DetailsProductAddItemVM()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl != null ? product.ImageUrl : GlobalConstants.DefaultProductImage,
                    HasExtras = product.HasExtras,
                    CategoryId = product.CategoryId,
                    Allergens = product.Allergens
                            .Select(c => new DetailsAllergenViewModel()
                            {
                                Id = c.AllergenId,
                                Name = c.Allergen.Name,
                                ImageUrl = c.Allergen.ImageUrl,
                            }).ToList(),
                    Sizes = size == null ? product.Sizes
                            .Select(c => new ProductSizeViewModel()
                            {
                                SizeId = c.Id,
                                SizeName = c.SizeName,
                                Price = c.Price,
                                Weight = c.Weight,
                                MaxProductsInPackage = c.MaxProductsInPackage,
                                PackagePrice = c.Package.Price,
                            }).ToList()
                            : new List<ProductSizeViewModel>()
                            {
                                new ProductSizeViewModel()
                                    {
                                        SizeId = size.Id,
                                        SizeName = size.SizeName,
                                        Price = size.Price,
                                        Weight = size.Weight,
                                        MaxProductsInPackage = size.MaxProductsInPackage,
                                        PackagePrice = size.Package.Price,
                                    },
                            },
                },
            };

            if (model.Product.HasExtras)
            {
                model.Extras = this.extrasService.All(isDeleted: false).ToList();
            }

            return model;
        }
    }
}
