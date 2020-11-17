namespace Tapas.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Addresses;

    [Authorize]
    public class AddressesController : BaseController
    {
        private readonly IAddressesService addressesService;
        private readonly UserManager<ApplicationUser> userManager;

        public AddressesController(
            IAddressesService addressesService,
            UserManager<ApplicationUser> userManager)
        {
            this.addressesService = addressesService;
            this.userManager = userManager;
        }

        // GET: Addresses
        public async Task<IActionResult> Index()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var model = this.addressesService.GetMyAddreses(user);
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
                model = await this.addressesService.GetAddressAsync(latitude, longitude);
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

            var addressId = await this.addressesService.CreateAddressAsync(user, model);
            this.TempData.Add(nameof(addressId), addressId);
            return this.Redirect("/Orders/Create");
        }
    }
}
