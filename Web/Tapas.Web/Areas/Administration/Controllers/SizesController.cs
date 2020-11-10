namespace Tapas.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Tapas.Services.Data.Contracts;

    public class SizesController : AdministrationController
    {
        private readonly ISizesService sizesService;
        private readonly IProductsService productsService;

        public SizesController(ISizesService sizesService, IProductsService productsService)
        {
            this.sizesService = sizesService;
            this.productsService = productsService;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Add(string productId)
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
                var model = this.sizesService.GetSizesOfProduct(productId);
                return this.View(model);
            }
            catch (System.Exception)
            {
                return this.BadRequest();
            }
        }

        [AllowAnonymous]
        public IActionResult GetSizeModel(string sizeId)
        {
            if (string.IsNullOrEmpty(sizeId))
            {
                return this.NotFound();
            }

            if (!int.TryParse(sizeId, out int id))
            {
                return this.NotFound();
            }

            if (!this.sizesService.ExistById(id))
            {
                return this.NotFound();
            }

            try
            {
                var model = this.sizesService.GetDetailModel(id);
                return this.View(model);
            }
            catch (System.Exception)
            {
                return this.NotFound();
            }
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult AddSizeModel(int index)
        {
            try
            {
                var model = this.sizesService.GetExtraSize(index);
                return this.View(model);
            }
            catch (System.Exception)
            {
                return this.NotFound();
            }
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Remove(int id)
        {
            try
            {
                this.sizesService.Remove(id);
                return this.Ok();
            }
            catch (System.Exception)
            {
                return this.NotFound();
            }
        }
    }
}
