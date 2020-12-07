namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Packages;

    public class PackagesService : IPackagesService
    {
        private readonly IDeletableEntityRepository<Package> packageRepository;

        public PackagesService(IDeletableEntityRepository<Package> packageRepository)
        {
            this.packageRepository = packageRepository;
        }

        public async Task AddAsync(PackageInputModel model)
        {
            await this.packageRepository
               .AddAsync(new Package()
               {
                   Name = model.Name,
                   Price = model.Price,
                   MistralCode = model.MistralCode,
                   MistralName = model.MistralName,
               });
            await this.packageRepository.SaveChangesAsync();
        }

        public IEnumerable<PackageViewModel> All()
        {
            return this.packageRepository.All().Select(x => new PackageViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
            }).ToList();
        }

        public async Task EditAsync(PackageViewModel packageViewModel)
        {
            var package = this.ExistPackageById(packageViewModel.Id);

            package.Name = packageViewModel.Name;
            package.Price = packageViewModel.Price;
            package.MistralCode = packageViewModel.MistralCode;
            package.MistralName = packageViewModel.MistralName;
            this.packageRepository.Update(package);
            await this.packageRepository.SaveChangesAsync();
        }

        public PackageViewModel GetPackageViewModelById(int packageId)
        {
            var model = this.packageRepository.All()
               .Where(x => x.Id == packageId)
               .Select(x => new PackageViewModel()
               {
                   Id = x.Id,
                   Name = x.Name,
                   Price = x.Price,
               })
               .FirstOrDefault();

            if (model is null)
            {
                throw new ArgumentException();
            }

            return model;
        }

        public async Task RemoveAsync(int packageId)
        {
            var package = this.ExistPackageById(packageId);

            this.packageRepository.Delete(package);
            await this.packageRepository.SaveChangesAsync();
        }

        private Package ExistPackageById(int packageId)
        {
            var package = this.packageRepository.All().FirstOrDefault(x => x.Id == packageId);

            if (package is null)
            {
                throw new ArgumentException();
            }

            return package;
        }
    }
}
