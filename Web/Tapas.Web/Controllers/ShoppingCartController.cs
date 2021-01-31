namespace Tapas.Web.Controllers
{
    using System;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Tapas.Common;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Orders;

    public class ShoppingCartController : BaseController
    {
        private readonly IShopingCartService cartService;
        private readonly ILogger<ShoppingCartController> logger;

        public ShoppingCartController(
            IShopingCartService cartService,
            ILogger<ShoppingCartController> logger)
        {
            this.cartService = cartService;
            this.logger = logger;
        }

        // GET
        public ActionResult Index()
        {
                return this.View(new OrderInpitModel());
        }

        // GET
        public ActionResult AddItem(string productId, int? sizeId = null)
        {
            try
            {
                var model = this.cartService.GetShopingModel(productId, sizeId);
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }
    }
}
