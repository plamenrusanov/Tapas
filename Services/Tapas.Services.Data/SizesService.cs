namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Packages;
    using Tapas.Web.ViewModels.Administration.Sizes;

    public class SizesService : ISizesService
    {
        private readonly IDeletableEntityRepository<MenuProduct> menuRepository;
        private readonly IDeletableEntityRepository<Package> packageRepository;
        private readonly IDeletableEntityRepository<ProductSize> sizeRepository;
        private readonly IDeletableEntityRepository<ShopingCartItem> shopingCartItemRepository;

        public SizesService(
            IDeletableEntityRepository<MenuProduct> menuRepository,
            IDeletableEntityRepository<Package> packageRepository,
            IDeletableEntityRepository<ProductSize> sizeRepository,
            IDeletableEntityRepository<ShopingCartItem> shopingCartItemRepository)
        {
            this.menuRepository = menuRepository;
            this.packageRepository = packageRepository;
            this.sizeRepository = sizeRepository;
            this.shopingCartItemRepository = shopingCartItemRepository;
        }

        private IEnumerable<PackageViewModel> AvailablePackages => this.packageRepository
          .All().Select(p => new PackageViewModel()
          {
              Id = p.Id,
              Name = p.Name,
          }).ToList();

        public bool ExistById(int sizeId)
        {
            return this.sizeRepository.All().Any(x => x.Id == sizeId);
        }

        public ProductSizeViewModel GetDetailModel(int sizeId)
        {
            var model = this.sizeRepository.All()
                .Where(x => x.Id == sizeId)
                .Select(x => new ProductSizeViewModel()
                {
                    SizeId = x.Id,
                    SizeName = x.SizeName,
                    Price = x.Price,
                    Weight = x.Weight,
                    MaxProductsInPackage = x.MaxProductsInPackage,
                    PackagePrice = x.Package.Price,
                    HasExtras = x.MenuProduct.HasExtras,
                }).FirstOrDefault();

            if (model is null)
            {
                throw new Exception();
            }

            return model;
        }

        public ExtraSize GetExtraSize(int index)
        {
            return new ExtraSize()
            {
                Index = index,
                AvailablePackages = this.AvailablePackages.ToList(),
            };
        }

        public IEnumerable<EditSizeModel> GetSizesOfProduct(string productId)
        {
            var sizes = this.menuRepository
                .All()
                .Where(x => x.Id == productId)
                .SelectMany(x => x.Sizes)
                .ToList();

            return sizes.Select(x => new EditSizeModel()
            {
                SizeId = x.Id,
                SizeName = x.SizeName,
                Price = x.Price,
                Weight = x.Weight,
                PackageId = x.PackageId,
                MaxProductsInPackage = x.MaxProductsInPackage,
                Packages = this.AvailablePackages.ToList(),
            }).ToList();
        }

        public string Remove(int id)
        {
            var productSize = this.sizeRepository.All().FirstOrDefault(x => x.Id == id);
            if (productSize is null)
            {
                throw new Exception();
            }

            var product = this.menuRepository.AllWithDeleted().FirstOrDefault(x => x.Id == productSize.MenuProductId);
            if (product is null)
            {
                throw new Exception();
            }

            if (product.Sizes.Count <= 1)
            {
                throw new Exception();
            }

            if (this.shopingCartItemRepository.All().Any(c => c.SizeId == id))
            {
                var listOfItems = this.shopingCartItemRepository.All().Where(x => x.SizeId == id).ToList();

                foreach (var item in listOfItems)
                {
                    this.shopingCartItemRepository.HardDelete(item);
                }

                this.shopingCartItemRepository.SaveChanges();
            }

            this.sizeRepository.HardDelete(productSize);
            this.sizeRepository.SaveChanges();
            return productSize.MenuProductId;
        }
    }
}
