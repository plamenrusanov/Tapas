namespace Tapas.Services.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Tapas.Common;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Contracts;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Allergens;
    using Tapas.Web.ViewModels.Administration.Categories;
    using Tapas.Web.ViewModels.Administration.Packages;
    using Tapas.Web.ViewModels.Administration.Products;
    using Tapas.Web.ViewModels.Administration.Sizes;

    public class ProductsService : IProductsService
    {
        private readonly IDeletableEntityRepository<MenuProduct> productsRepo;
        private readonly ICloudService cloudService;
        private readonly IRepository<Allergen> allergensRepository;
        private readonly IRepository<Category> categoriesRepository;
        private readonly IDeletableEntityRepository<Package> packageRepository;
        private readonly IDeletableEntityRepository<ProductSize> sizeRepository;
        private readonly IDeletableEntityRepository<AllergensProducts> allergensProductsRepository;
        private readonly IDeletableEntityRepository<ShopingCartItem> itemRepository;
        private string productZeroSizes = "Продукта не може да съществува без размер.";

        public ProductsService(
            IDeletableEntityRepository<MenuProduct> productsRepo,
            ICloudService cloudService,
            IRepository<Allergen> allergensRepository,
            IRepository<Category> categoriesRepository,
            IDeletableEntityRepository<Package> packageRepository,
            IDeletableEntityRepository<ProductSize> sizeRepository,
            IDeletableEntityRepository<AllergensProducts> allergensProductsRepository,
            IDeletableEntityRepository<ShopingCartItem> itemRepository)
        {
            this.productsRepo = productsRepo;
            this.cloudService = cloudService;
            this.allergensRepository = allergensRepository;
            this.categoriesRepository = categoriesRepository;
            this.packageRepository = packageRepository;
            this.sizeRepository = sizeRepository;
            this.allergensProductsRepository = allergensProductsRepository;
            this.itemRepository = itemRepository;
        }

        private List<PackageViewModel> GetAvailablePackigesVM => this.packageRepository
               .All()
               .Select(x => new PackageViewModel()
               {
                   Id = x.Id,
                   Name = x.Name,
                   Price = x.Price,
               }).ToList();

        public async Task AddAsync(ProductInputViewModel model)
        {
            var product = new MenuProduct()
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                Description = model.Description,
                HasExtras = model.HasExtras,
                Sizes = new List<ProductSize>()
                {
                },
            };

            foreach (var item in model.Allergens)
            {
                product.Allergens.Add(new AllergensProducts()
                {
                    ProductId = product.Id,
                    AllergenId = item,
                });
            }

            foreach (var size in model.Sizes)
            {
                if (string.IsNullOrEmpty(size.SizeName))
                {
                    continue;
                }

                product.Sizes.Add(
                  new ProductSize()
                  {
                      SizeName = size.SizeName,
                      PackageId = size.PackageId,
                      Price = size.Price,
                      Weight = size.Weight,
                      MaxProductsInPackage = size.MaxProductsInPackage,
                      MenuProductId = product.Id,
                      MistralCode = size.MistralCode,
                      MistralName = size.MistralName,
                  });
            }

            if (model.Image != null)
            {
                product.ImageUrl = await this.cloudService.UploadImageFromForm(model.Image);
            }

            await this.productsRepo.AddAsync(product);
            await this.productsRepo.SaveChangesAsync();
        }

        public bool ExistProductById(string productId)
        {
            return this.productsRepo.AllWithDeleted().Any(x => x.Id == productId);
        }

        public DetailsProductViewModel GetDetailsProductById(string productId)
        {
            var product = this.CheckNullExistsReturnProduct(productId);

            var model = new DetailsProductViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl != null ? product.ImageUrl : GlobalConstants.DefaultProductImage,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Allergens = product.Allergens
                        .Select(c => new DetailsAllergenViewModel()
                        {
                            Id = c.AllergenId,
                            Name = c.Allergen.Name,
                            ImageUrl = c.Allergen.ImageUrl,
                        }).ToList(),
            };
            return model;
        }

        public EditProductModel GetEditProductById(string productId)
        {
            var product = this.CheckNullExistsReturnProduct(productId);

            var packages = this.GetAvailablePackigesVM;

            var model = new EditProductModel()
            {
                Id = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                Description = product.Description,
                HasExtras = product.HasExtras,
                AvailablePackages = packages,
            };
            model.Allergens = this.allergensRepository
                 .All()
                 .ToList()
                 .Select(c => new SelectListItem()
                 {
                     Value = c.Id,
                     Text = c.Name,
                     Selected = product.Allergens.Any(a => a.AllergenId == c.Id),
                 }).ToList();
            model.AvailableCategories = this.categoriesRepository
                .All()
                .Select(c => new SelectListItem()
                {
                    Value = c.Id,
                    Text = c.Name,
                    Selected = product.CategoryId == c.Id,
                }).ToList();
            model.Sizes = this.sizeRepository
                .All()
                .Where(c => c.MenuProductId == product.Id)
                .Select(c => new EditProductSizeModel()
                {
                    SizeId = c.Id,
                    SizeName = c.SizeName,
                    Price = c.Price,
                    Weight = c.Weight,
                    PackageId = c.PackageId,
                    MaxProductsInPackage = c.MaxProductsInPackage,
                    MistralCode = c.MistralCode,
                    MistralName = c.MistralName,
                }).ToList();
            return model;
        }

        public async Task EditProductAsync(EditProductModel model)
        {
            if (model.Image != null)
            {
                model.ImageUrl = await this.cloudService.UploadImageFromForm(model.Image);
            }

            var product = this.productsRepo.AllWithDeleted()
                .Where(x => x.Id == model.Id)
                .FirstOrDefault();
            product.Name = model.Name;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.Description = model.Description;
            product.HasExtras = model.HasExtras;

            foreach (var size in model.Sizes)
            {
                if (size.SizeId == 0)
                {
                    product.Sizes.Add(new ProductSize()
                    {
                        SizeName = size.SizeName,
                        Price = size.Price,
                        Weight = size.Weight,
                        MaxProductsInPackage = size.MaxProductsInPackage,
                        PackageId = size.PackageId,
                        MenuProductId = product.Id,
                        MistralCode = size.MistralCode,
                        MistralName = size.MistralName,
                    });
                }
                else
                {
                    var s = product.Sizes.Where(x => x.Id == size.SizeId).FirstOrDefault();
                    if (s is null)
                    {
                        continue;
                    }

                    s.SizeName = size.SizeName;
                    s.Price = size.Price;
                    s.Weight = size.Weight;
                    s.MaxProductsInPackage = size.MaxProductsInPackage;
                    s.MenuProductId = product.Id;
                    s.PackageId = size.PackageId;
                    s.MistralCode = size.MistralCode;
                    s.MistralName = size.MistralName;
                }
            }

            foreach (var item in model.Allergens)
            {
                if (item.Selected)
                {
                    if (!product.Allergens.Any(x => x.AllergenId == item.Value))
                    {
                        product.Allergens
                            .Add(new AllergensProducts()
                            {
                                ProductId = product.Id,
                                AllergenId = item.Value,
                            });
                    }
                }
                else
                {
                    if (product.Allergens.Any(x => x.AllergenId == item.Value))
                    {
                        var allergenProduct = product.Allergens.FirstOrDefault(x => x.AllergenId == item.Value);
                        this.allergensProductsRepository.HardDelete(allergenProduct);
                    }
                }
            }

            if (product.Sizes.Count == 0)
            {
                throw new ArgumentException(this.productZeroSizes);
            }

            await this.allergensProductsRepository.SaveChangesAsync();
            await this.productsRepo.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(string productId)
        {
            var product = this.CheckNullExistsReturnProduct(productId);
            this.productsRepo.Delete(product);
            await this.productsRepo.SaveChangesAsync();
        }

        public ICollection<DetailsProductViewModel> GetAllProducts(bool isDeleted)
        {
            return this.productsRepo.AllWithDeleted()
                .Where(x => x.IsDeleted == isDeleted)
                .Select(x => new DetailsProductViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    CategoryName = x.Category.Name,
                    IsDeleted = x.IsDeleted,
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        public void Activate(string productId)
        {
            var product = this.CheckNullExistsReturnProduct(productId);
            product.IsDeleted = false;
            this.productsRepo.SaveChanges();
        }

        public ProductsIndexViewModel CategoryWhitProducts(string categoryId = null)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            var model = new ProductsIndexViewModel()
            {
                Categories = this.categoriesRepository
                .AllAsNoTracking()
                .Select(x => new CategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList(),
            };

            if (this.categoriesRepository.All().Any(x => x.Id == categoryId))
            {
                model.Products = this.productsRepo
                  .All()
                  .Where(x => x.CategoryId == categoryId)
                  .Select(x => new MenuProductViewModel()
                  {
                      Id = x.Id,
                      Name = x.Name,
                      ImageUrl = x.ImageUrl != null ? x.ImageUrl : GlobalConstants.DefaultProductImage,
                      IsOneSize = x.Sizes.Count == 1,
                      Sizes = x.Sizes.Select(s => new ProductSizeViewModel()
                      {
                          SizeName = s.SizeName,
                          Price = s.Price,
                      }).ToList(),
                      Weight = x.Sizes.Count == 1 ? x.Sizes.FirstOrDefault().Weight : default,
                      Price = x.Sizes.Count == 1 ? x.Sizes.FirstOrDefault().Price : default,
                      Rating = (byte)Math.Round((double)this.itemRepository
                                .All()
                                .Where(r => r.ProductId == x.Id && r.Rating > 0)
                                .Select(p => (int)p.Rating)
                                .Sum() /
                                this.itemRepository
                                .All()
                                .Where(r => r.ProductId == x.Id && r.Rating > 0)
                                .ToList()
                                .Count),
                  })
                  .ToList()
                  .OrderByDescending(x => x.Rating);
                return model;
            }

            model.Products = this.productsRepo
                .All()
                .Select(x => new MenuProductViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageUrl != null ? x.ImageUrl : GlobalConstants.DefaultProductImage,
                    IsOneSize = x.Sizes.Count == 1,
                    Sizes = x.Sizes.Select(s => new ProductSizeViewModel()
                    {
                        SizeName = s.SizeName,
                        Price = s.Price,
                    }).ToList(),
                    Weight = x.Sizes.Count == 1 ? x.Sizes.FirstOrDefault().Weight : default,
                    Price = x.Sizes.Count == 1 ? x.Sizes.FirstOrDefault().Price : default,
                    Rating = (byte)Math.Round((double)this.itemRepository
                                .All()
                                .Where(r => r.ProductId == x.Id && r.Rating > 0)
                                .Select(p => (int)p.Rating)
                                .Sum() /
                                this.itemRepository
                                .All()
                                .Where(r => r.ProductId == x.Id && r.Rating > 0)
                                .ToList()
                                .Count),
                })
                .ToList()
                .OrderByDescending(x => x.Rating)
                .Take(12);
            st.Stop();
            Console.WriteLine(st.Elapsed);
            return model;
        }

        private MenuProduct CheckNullExistsReturnProduct(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException(paramName: nameof(productId));
            }

            var menuProduct = this.productsRepo
                .AllWithDeleted()
                .Where(x => x.Id == productId)
                .FirstOrDefault();

            if (menuProduct is null)
            {
                throw new ArgumentException(string.Format(ExceptionMessages.NotExists, nameof(menuProduct)));
            }

            return menuProduct;
        }
    }
}
