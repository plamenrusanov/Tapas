namespace Tapas.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Addreses;

    [Authorize]
    public class AddresesController : BaseController
    {
        private readonly IAddresesService addresesService;
        private readonly UserManager<ApplicationUser> userManager;

        public AddresesController(
            IAddresesService addresesService,
            UserManager<ApplicationUser> userManager)
        {
            this.addresesService = addresesService;
            this.userManager = userManager;
        }

        // GET: Addreses
        public async Task<IActionResult> Index()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var model = this.addresesService.GetMyAddreses(user);
            return this.View(model);
        }

        public async Task<AddressInputModel> GetAddressFromLocation(string latitude, string longitude)
        {
            if (string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
            {
                return null;
            }

            AddressInputModel model = new AddressInputModel();
            try
            {
                model = await this.addresesService.GetAddressAsync(latitude, longitude);
            }
            catch (Exception)
            {
                return null;
            }

            return model;
        }

        // GET: Addreses/Create
        public IActionResult Create()
        {
            var model = new AddressInputModel();

            return this.View(model);
        }

        // POST: Addreses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddressInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.addresesService.CreateAddressAsync(user, model);

            return this.Redirect("/Orders/Create");
        }
    }
}
