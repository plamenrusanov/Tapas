namespace Tapas.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Tapas.Common;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.CateringFood;

    public class CateringFoodController : AdministrationController
    {
        private readonly IAllergensService allergensService;
        private readonly ICateringFoodService cateringFoodService;
        private readonly IPackagesService packagesService;
        private readonly ILogger<CateringFoodController> logger;

        public CateringFoodController(
            IAllergensService allergensService,
            ICateringFoodService cateringFoodService,
            IPackagesService packagesService,
            ILogger<CateringFoodController> logger)
        {
            this.allergensService = allergensService;
            this.cateringFoodService = cateringFoodService;
            this.packagesService = packagesService;
            this.logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = this.cateringFoodService.GetAll();
            return this.View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = this.cateringFoodService.CreateInputModel();
            return this.View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (!this.ModelState.IsValid)
            {
                model.AvailablePackages = this.packagesService.All().ToList();
                model.AvailableAllergens = this.allergensService.All().ToList();
                return this.View(model);
            }

            try
            {
                await this.cateringFoodService.AddCateringFoodAsync(model);
                return this.RedirectToAction("Index");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [AllowAnonymous]
        public IActionResult Details(string id)
        {
            try
            {
                var model = this.cateringFoodService.GetDetailsById(id);
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        public IActionResult Edit(string id)
        {
            try
            {
                var model = this.cateringFoodService.GetEditModel(id);
                return this.View(model);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.InnerException.Message, e.InnerException.StackTrace);
                }

                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Edit(EditCateringFoodModel model)
        {
            if (!this.ModelState.IsValid)
            {
                model.AvailablePackages = this.packagesService.All().ToList();
                return this.View(model);
            }

            try
            {
                this.cateringFoodService.SetEditModel(model);
                return this.RedirectToAction("Index");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await this.cateringFoodService.Delete(id);
                return this.RedirectToAction("Index");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        public IActionResult GetDeletedProducts()
        {
            try
            {
                var model = this.cateringFoodService.GetDeletedProducts();
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        public async Task<IActionResult> Activate(string productId)
        {
            try
            {
                await this.cateringFoodService.ActivateAsync(productId);
                return this.RedirectToAction(actionName: "Details", routeValues: new { id = productId });
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }
    }
}
