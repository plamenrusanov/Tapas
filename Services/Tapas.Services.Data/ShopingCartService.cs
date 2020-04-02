﻿namespace Tapas.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Allergens;
    using Tapas.Web.ViewModels.Administration.AllergensProducts;
    using Tapas.Web.ViewModels.Administration.Products;
    using Tapas.Web.ViewModels.ShopingCart;
    using Tapas.Web.ViewModels.ShopingCartItems;

    public class ShopingCartService : IShopingCartService
    {
        private readonly IDeletableEntityRepository<ShopingCart> cartRepository;
        private readonly IDeletableEntityRepository<Product> productRepository;

        public ShopingCartService(
            IDeletableEntityRepository<ShopingCart> cartRepository,
            IDeletableEntityRepository<Product> productRepository)
        {
            this.cartRepository = cartRepository;
            this.productRepository = productRepository;
        }

        public async Task CreateShopingCartAsync(string userId)
        {
            await this.cartRepository.AddAsync(new ShopingCart() { ApplicationUserId = userId });
            await this.cartRepository.SaveChangesAsync();
        }

        public ShopingCartViewModel GetShopingCart(ApplicationUser user)
        {
            return this.cartRepository.All()
                .Where(x => x.Id == user.ShopingCart.Id)
                .Select(x => new ShopingCartViewModel()
                {
                    Id = x.Id,
                    UserId = x.ApplicationUserId,
                    TotalPrice = x.CartItems.Sum(c => (c.Product.Price * c.Quantity)),
                    ShopingItems = x.CartItems.Select(c => new ShopingItemsViewModel()
                    {
                        Id = c.Id,
                        ProductId = c.ProductId,
                        ProductName = c.Product.Name,
                        ProductPrice = c.Product.Price,
                        Quantity = c.Quantity,
                    }).ToList(),
                }).FirstOrDefault();
        }

        public AddItemViewModel GetShopingModel(ApplicationUser user, string productId)
        {
            return new AddItemViewModel()
            {
                Product = this.productRepository
                    .All()
                    .Where(x => x.Id == productId)
                    .Select(x => new DetailsProductViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        ImageUrl = x.ImageUrl,
                        Allergens = x.Allergens
                            .Select(c => new DetailsAllergenViewModel()
                            {
                                Id = c.AllergenId,
                                Name = c.Allergen.Name,
                                ImageUrl = c.Allergen.ImageUrl,
                            }).ToList(),
                    }).FirstOrDefault(),
                ShopingCart = this.GetShopingCart(user),
            };
        }
    }
}
