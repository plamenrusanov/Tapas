namespace Tapas.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Tapas.Common;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Products;

    public class ProductsController : AdministrationController
    {
        private const string Active = "Активни";
        private const string Inactive = "Неактивни";
        private readonly IProductsService productsService;
        private readonly ICategoriesService categoriesService;
        private readonly IAllergensService allergensService;
        private readonly IPackagesService packagesService;
        private readonly ILogger logger;

        public ProductsController(
            IProductsService productsService,
            ICategoriesService categoriesService,
            IAllergensService allergensService,
            IPackagesService packagesService,
            ILogger<ProductsController> logger)
        {
            this.productsService = productsService;
            this.categoriesService = categoriesService;
            this.allergensService = allergensService;
            this.packagesService = packagesService;
            this.logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            this.ViewData["Title"] = "Меню";
            var homeIndexViewModel = this.productsService.CategoryWhitProducts();
            return this.View(homeIndexViewModel);
        }

        [AllowAnonymous]
        public IActionResult GetProductsByCategory(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return this.NotFound();
            }

            if (!this.categoriesService.ExistCategoryById(categoryId))
            {
                return this.NotFound();
            }

            try
            {
                this.ViewData["Title"] = this.categoriesService.GetCategoryNameById(categoryId);
                var homeIndexViewModel = this.productsService.CategoryWhitProducts(categoryId);
                return this.View(viewName: "Index", model: homeIndexViewModel);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User?.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new ProductInputViewModel()
            {
                AvailableCategories = this.categoriesService.All().ToList(),
                AvailableAllergens = this.allergensService.All().ToList(),
                AvailablePackages = this.packagesService.All().ToList(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm]ProductInputViewModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                inputModel.AvailableCategories = this.categoriesService.All().ToList();
                inputModel.AvailableAllergens = this.allergensService.All().ToList();
                inputModel.AvailablePackages = this.packagesService.All().ToList();
                return this.View(inputModel);
            }

            try
            {
                await this.productsService.AddAsync(inputModel);

                return this.Redirect("/");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        public IActionResult Details(string productId)
        {
            try
            {
                var viewModel = this.productsService.GetDetailsProductById(productId);
                return this.View(viewModel);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        public IActionResult Edit(string productId)
        {
            try
            {
                var viewModel = this.productsService.GetEditProductById(productId);
                return this.View(viewModel);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductModel model)
        {
            if (!this.ModelState.IsValid)
            {
                model.AvailableCategories = this.categoriesService
                    .All()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id,
                        Selected = x.Id == model.CategoryId,
                    })
                    .ToList();
                model.AvailablePackages = this.packagesService.All().ToList();
                return this.View(model);
            }

            try
            {
                await this.productsService.EditProductAsync(model);
                return this.RedirectToAction("GetProducts", false);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        public async Task<IActionResult> OnDelete(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return this.NotFound();
            }

            if (!this.productsService.ExistProductById(productId))
            {
                return this.NotFound();
            }

            try
            {
                await this.productsService.DeleteProductAsync(productId);
                return this.RedirectToAction("GetProducts", false);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.RedirectToAction("GetProducts", true);
            }
        }

        public IActionResult GetProducts(bool isDeleted = false)
        {
            this.ViewData["Title"] = isDeleted ? Inactive : Active;
            this.ViewData["IsDeleted"] = isDeleted;
            try
            {
                var model = this.productsService.GetAllProducts(isDeleted);
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        public IActionResult Activate(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return this.NotFound();
            }

            if (!this.productsService.ExistProductById(productId))
            {
                return this.NotFound();
            }

            try
            {
                this.productsService.Activate(productId);

                return this.RedirectToAction("GetProducts", false);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }
    }
}
