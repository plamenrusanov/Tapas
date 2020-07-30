namespace Tapas.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Tapas.Common;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.ShopingCart;

    [Authorize]
    public class ShopingCartController : Controller
    {
        private readonly IShopingCartService cartService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProductsService productsService;
        private readonly ILogger<ShopingCartController> logger;

        public ShopingCartController(
            IShopingCartService cartService,
            UserManager<ApplicationUser> userManager,
            IProductsService productsService,
            ILogger<ShopingCartController> logger)
        {
            this.cartService = cartService;
            this.userManager = userManager;
            this.productsService = productsService;
            this.logger = logger;
        }

        // GET
        public async Task<ActionResult> Index()
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);
                var cart = this.cartService.GetShopingCart(user);
                return this.View(cart);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        // GET
        public async Task<ActionResult> AddItem(string productId)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);
                var model = this.cartService.GetShopingModel(productId);
                model.ShopingCartId = user.ShopingCart.Id;
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem([FromForm] AddItemViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewName: nameof(this.AddItem), model: model.Product.Id);
            }

            try
            {
                // var user = await this.userManager.GetUserAsync(this.User);
                // model.ShopingCartId = user.ShopingCartId;
                await this.cartService.AddItemAsync(model);
                return this.RedirectToAction("Index");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        // GET
        public async Task<ActionResult> DeleteProductItem(int itemId, string shopingCartId)
        {
            if (this.User == null)
            {
                return this.RedirectToPage(GlobalConstants.LoginPageRoute);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            if (shopingCartId == null || user.ShopingCart.Id != shopingCartId)
            {
                shopingCartId = user.ShopingCart.Id;
            }

            if (!user.ShopingCart.CartItems.Any(x => x.Id == itemId))
            {
                return this.NotFound();
            }

            try
            {
                await this.cartService.DeleteItemAsync(itemId, shopingCartId);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }

            if (this.Request.Headers.ContainsKey(GlobalConstants.RefererHeader))
            {
                var refererValue = default(Microsoft.Extensions.Primitives.StringValues);
                bool result = this.Request.Headers.TryGetValue(GlobalConstants.RefererHeader, out refererValue);
                if (result)
                {
                    var uri = new Uri(refererValue);
                    return this.LocalRedirect(uri.PathAndQuery);
                }
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        public string GetDescription(int id)
        {
            try
            {
                var desc = this.cartService.GetDescription(id);
                if (desc is null)
                {
                    desc = string.Empty;
                }

                return desc;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return string.Empty;
            }
        }

        public async Task<string> SetDescription(int id, string message)
        {
            if (message != null && message.Length > GlobalConstants.DescriptionMaxLength)
            {
                return string.Empty;
            }

            try
            {
                await this.cartService.SetDescriptionAsync(id, message);
                return "Ok";
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return string.Empty;
            }
        }
    }
}
