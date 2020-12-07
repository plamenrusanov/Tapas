namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Tapas.Common;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Contracts;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Allergens;
    using Tapas.Web.ViewModels.Administration.CateringFood;
    using Tapas.Web.ViewModels.Administration.Sizes;

    public class CateringFoodService : ICateringFoodService
    {
        private readonly IDeletableEntityRepository<CateringProduct> cateringRepository;
        private readonly IDeletableEntityRepository<AllergensProducts> allergensProductsRepository;
        private readonly IAllergensService allergensService;
        private readonly IPackagesService packagesService;
        private readonly ICloudService cloudService;

        public CateringFoodService(
            IDeletableEntityRepository<CateringProduct> cateringRepository,
            IDeletableEntityRepository<AllergensProducts> allergensProductsRepository,
            IAllergensService allergensService,
            IPackagesService packagesService,
            ICloudService cloudService)
        {
            this.cateringRepository = cateringRepository;
            this.allergensProductsRepository = allergensProductsRepository;
            this.allergensService = allergensService;
            this.packagesService = packagesService;
            this.cloudService = cloudService;
        }

        // Post Administration/CateringFood/Create
        public async Task AddCateringFoodAsync(CreateModel model)
        {
            var cateringProduct = new CateringProduct()
            {
                Name = model.Name,
                Description = model.Description,
                NumberOfBits = model.NumberOfBits,
                Size = new ProductSize()
                {
                    SizeName = model.Size.SizeName,
                    PackageId = model.Size.PackageId,
                    Price = model.Size.Price,
                    Weight = model.Size.Weight,
                    MaxProductsInPackage = model.Size.MaxProductsInPackage,
                },
            };

            foreach (var item in model.Allergens)
            {
                cateringProduct.Allergens.Add(new AllergensProducts()
                {
                    ProductId = cateringProduct.Id,
                    AllergenId = item,
                });
            }

            if (model.Image != null)
            {
                cateringProduct.ImageUrl = await this.cloudService.UploadImageFromForm(model.Image);
            }

            await this.cateringRepository.AddAsync(cateringProduct);
            await this.cateringRepository.SaveChangesAsync();
        }

        // Administration/CateringFood/Create
        public CreateModel CreateInputModel()
        {
            return new CreateModel()
            {
                AvailableAllergens = this.allergensService.All().ToList(),
                AvailablePackages = this.packagesService.All().ToList(),
                Size = new InputProductSizeModel() { SizeName = "Default" },
            };
        }

        // Administration/CateringFood/Index
        public IEnumerable<IndexCateringFoodViewModel> GetAll()
        {
            return this.cateringRepository.All()
                .Select(x => new IndexCateringFoodViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    NumberOfBits = (int)x.NumberOfBits,
                    Allergens = x.Allergens
                        .Select(a => new DetailsAllergenViewModel()
                        {
                            Id = a.Allergen.Id,
                            Name = a.Allergen.Name,
                            ImageUrl = a.Allergen.ImageUrl,
                        }).ToList(),
                    Size = new ProductSizeViewModel()
                    {
                        SizeId = x.Size.Id,
                        SizeName = x.Size.SizeName,
                        Price = x.Size.Price,
                        Weight = x.Size.Weight,
                        MaxProductsInPackage = x.Size.MaxProductsInPackage,
                        PackagePrice = x.Size.Package.Price,
                    },
                }).ToList();
        }

        // Administration/CateringFood/Details
        public DetailsCateringFoodViewModel GetDetailsById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            var product = this.cateringRepository
                .All()
                .Where(x => x.Id == id)
                .Select(x => new DetailsCateringFoodViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    NumberOfBits = x.NumberOfBits,
                    Size = new ProductSizeViewModel()
                    {
                        SizeId = x.Size.Id,
                        SizeName = x.Size.SizeName,
                        Price = x.Size.Price,
                        Weight = x.Size.Weight,
                        PackagePrice = x.Size.Package.Price,
                        MaxProductsInPackage = x.Size.MaxProductsInPackage,
                    },
                    Allergens = x.Allergens.Select(a => new DetailsAllergenViewModel()
                    {
                        Id = a.AllergenId,
                        Name = a.Allergen.Name,
                        ImageUrl = a.Allergen.ImageUrl,
                    }).ToList(),
                }).FirstOrDefault();

            if (product is null)
            {
                throw new ArgumentException();
            }

            return product;
        }

        // Administration/CateringFood/Edit
        public EditCateringFoodModel GetEditModel(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            try
            {
                var cateringProduct = this.cateringRepository
                    .All()
                    .Where(x => x.Id == id)
                    .FirstOrDefault();
                if (cateringProduct is null)
                {
                    throw new ArgumentNullException(string.Format(ExceptionMessages.NotExists, nameof(cateringProduct)));
                }

                var model = new EditCateringFoodModel()
                {
                    Id = cateringProduct.Id,
                    Name = cateringProduct.Name,
                    ImageUrl = cateringProduct.ImageUrl,
                    NumberOfBits = cateringProduct.NumberOfBits,
                    Description = cateringProduct.Description,
                    Size = new EditProductSizeModel()
                    {
                        SizeId = cateringProduct.Size.Id,
                        SizeName = cateringProduct.Size.SizeName,
                        Price = cateringProduct.Size.Price,
                        Weight = cateringProduct.Size.Weight,
                        PackageId = cateringProduct.Size.PackageId,
                        MaxProductsInPackage = cateringProduct.Size.MaxProductsInPackage,
                    },
                    AvailablePackages = this.packagesService.All().ToList(),
                };

                model.Allergens = this.allergensService
                           .All()
                           .Select(c => new SelectListItem()
                           {
                               Value = c.Id,
                               Text = c.Name,
                               Selected = cateringProduct.Allergens.Any(a => a.AllergenId == c.Id) ? true : false,
                           }).ToList();

                return model;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        // Administration/CateringFood/Edit
        public async Task SetEditModel(EditCateringFoodModel model)
        {
            var cateringProduct = this.cateringRepository
                .All()
                .Where(x => x.Id == model.Id)
                .Include(x => x.Size)
                .Include(x => x.Allergens)
                .FirstOrDefault();
            if (cateringProduct is null)
            {
                throw new ArgumentException(string.Format(ExceptionMessages.NotExists, nameof(cateringProduct)));
            }

            if (model.Image != null)
            {
                model.ImageUrl = await this.cloudService.UploadImageFromForm(model.Image);
            }

            cateringProduct.Name = model.Name;
            cateringProduct.NumberOfBits = model.NumberOfBits;
            cateringProduct.Description = model.Description;
            cateringProduct.ImageUrl = model.ImageUrl;
            var size = cateringProduct.Size;

            size.Id = model.Size.SizeId;
            size.SizeName = model.Size.SizeName;
            size.MaxProductsInPackage = model.Size.MaxProductsInPackage;
            size.CareringProductId = cateringProduct.Id;
            size.PackageId = model.Size.PackageId;
            size.Price = model.Size.Price;
            size.Weight = model.Size.Weight;

            foreach (var item in model.Allergens)
            {
                if (item.Selected)
                {
                    if (!cateringProduct.Allergens.Any(x => x.AllergenId == item.Value))
                    {
                        cateringProduct.Allergens
                            .Add(new AllergensProducts()
                            {
                                ProductId = cateringProduct.Id,
                                AllergenId = item.Value,
                            });
                    }
                }
                else
                {
                    if (cateringProduct.Allergens.Any(x => x.AllergenId == item.Value))
                    {
                        var allergenProduct = cateringProduct.Allergens.FirstOrDefault(x => x.AllergenId == item.Value);
                        this.allergensProductsRepository.HardDelete(allergenProduct);
                    }
                }
            }

            this.allergensProductsRepository.SaveChanges();
            this.cateringRepository.SaveChanges();
        }

        // Administration/CateringFood/Delete
        public async Task Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            var cateringProduct = this.cateringRepository
                .All()
                .Where(x => x.Id == id)
                .FirstOrDefault();
            if (cateringProduct is null)
            {
                throw new ArgumentNullException(string.Format(ExceptionMessages.NotExists, nameof(cateringProduct)));
            }

            this.cateringRepository.Delete(cateringProduct);
            await this.cateringRepository.SaveChangesAsync();
        }

        public IEnumerable<DeletedCateringProducts> GetDeletedProducts()
        {
            var model = this.cateringRepository
                .AllWithDeleted()
                .Where(x => x.IsDeleted == true)
                .Select(x => new DeletedCateringProducts()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Size.Price,
                    Weight = x.Size.Weight,
                })
                .OrderBy(x => x.Name)
                .ToList();
            return model;
        }

        // Administration/CateringFood/Activate
        public async Task ActivateAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException();
            }

            var cateringProduct = this.cateringRepository
                .AllWithDeleted()
                .Where(x => x.Id == productId)
                .FirstOrDefault();
            if (cateringProduct is null)
            {
                throw new ArgumentNullException(string.Format(ExceptionMessages.NotExists, nameof(cateringProduct)));
            }

            this.cateringRepository.Undelete(cateringProduct);
            await this.cateringRepository.SaveChangesAsync();
        }
    }
}
