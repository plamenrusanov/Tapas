namespace Tapas.Web.Areas.Administration.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Categories;

    public class CategoriesController : AdministrationController
    {
        private const string CategoryExist = "Category already exist.";
        private readonly ICategoriesService categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        public IActionResult Index()
        {
            var categories = this.categoriesService.All();
            return this.View(categories);
        }

        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryInputViewModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            if (this.categoriesService.ExistCategoryByName(inputModel.Name))
            {
                this.ModelState.AddModelError(string.Empty, CategoryExist);
                return this.View();
            }

            try
            {
                await this.categoriesService.AddAsync(inputModel.Name);

                return this.RedirectToAction("Index");
            }
            catch (System.Exception)
            {
                return this.BadRequest();
            }
        }

        public IActionResult Edit(string categoryId)
        {
            try
            {
                var category = this.categoriesService.GetCategoryViewModelById(categoryId);
                if (category == null)
                {
                    return this.NotFound();
                }

                return this.View(category);
            }
            catch (System.Exception)
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Edit(CategoryViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            try
            {
                this.categoriesService.Edit(viewModel);

                return this.RedirectToAction("Index");
            }
            catch (System.Exception)
            {
                return this.BadRequest();
            }
        }

        public IActionResult Details(string categoryId)
        {
            if (!this.categoriesService.ExistCategoryById(categoryId))
            {
                return this.NotFound();
            }

            try
            {
                var category = this.categoriesService.GetCategoryViewModelById(categoryId);

                return this.View(category);
            }
            catch (System.Exception)
            {
                return this.BadRequest();
            }
        }

        public IActionResult Delete(string categoryId)
        {
            if (!this.categoriesService.ExistCategoryById(categoryId))
            {
                return this.NotFound();
            }

            try
            {
                var category = this.categoriesService.GetCategoryViewModelById(categoryId);

                return this.View(category);
            }
            catch (System.Exception)
            {
                return this.BadRequest();
            }
        }

        public IActionResult OnDelete(string categoryId)
        {
            if (!this.categoriesService.ExistCategoryById(categoryId))
            {
                return this.NotFound();
            }

            try
            {
                this.categoriesService.Remove(categoryId);

                return this.RedirectToAction("Index");
            }
            catch (System.Exception)
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePositions(string models)
        {
            var m = JsonConvert.DeserializeObject<List<CategoryViewModel>>(models);
            await this.categoriesService.SavePositions(m);
            return this.Ok();
        }
    }
}
