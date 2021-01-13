namespace Tapas.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Tapas.Common;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Packages;

    [Authorize(Roles = GlobalConstants.AdministratorName)]
    public class PackagesController : AdministrationController
    {
        private readonly IPackagesService packagesService;

        public PackagesController(IPackagesService packagesService)
        {
            this.packagesService = packagesService;
        }

        public IActionResult Index()
        {
            if (this.User == null)
            {
                return this.RedirectToPage("/Account/Login");
            }

            try
            {
                var packages = this.packagesService.All();
                return this.View(packages);
            }
            catch (Exception)
            {
                return this.NotFound();
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(PackageInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            try
            {
                await this.packagesService.AddAsync(inputModel);

                return this.RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return this.NotFound();
            }
        }

        public IActionResult Edit(int packageId)
        {
            try
            {
                var package = this.packagesService.GetPackageViewModelById(packageId);
                return this.View(package);
            }
            catch (Exception)
            {
                return this.NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PackageViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            try
            {
                await this.packagesService.EditAsync(viewModel);
                return this.RedirectToAction("Index");
            }
            catch (Exception)
            {
                return this.NotFound();
            }
        }

        public IActionResult Details(int packageId)
        {
            try
            {
                var package = this.packagesService.GetPackageViewModelById(packageId);

                return this.View(package);
            }
            catch (Exception)
            {
                return this.NotFound();
            }
        }

        public IActionResult Delete(int packageId)
        {
            try
            {
                var package = this.packagesService.GetPackageViewModelById(packageId);

                return this.View(package);
            }
            catch (Exception)
            {
                return this.NotFound();
            }
        }

        public async Task<IActionResult> OnDelete(int packageId)
        {
            try
            {
                await this.packagesService.RemoveAsync(packageId);

                return this.RedirectToAction("Index");
            }
            catch (Exception)
            {
                return this.NotFound();
            }
        }
    }
}
