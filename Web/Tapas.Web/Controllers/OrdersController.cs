namespace Tapas.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Tapas.Common;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.Hubs;
    using Tapas.Web.ViewModels.Orders;

    [Authorize]
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOrdersService ordersService;
        private readonly IHubContext<OrderHub> hubAdmin;
        private readonly IHubContext<UserOrdersHub> hubUser;
        private readonly ILogger<OrdersController> logger;

        public OrdersController(
            UserManager<ApplicationUser> userManager,
            IOrdersService ordersService,
            IHubContext<OrderHub> hubAdmin,
            IHubContext<UserOrdersHub> hubUser,
            ILogger<OrdersController> logger)
        {
            this.userManager = userManager;
            this.ordersService = ordersService;
            this.hubAdmin = hubAdmin;
            this.hubUser = hubUser;
            this.logger = logger;
        }

        // Orders/Index
        [Authorize(Roles = "Operator")]
        public IActionResult Index()
        {
            var orders = this.ordersService.GetDailyOrders();
            return this.View(orders);
        }

        // Orders/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var user = await this.userManager.GetUserAsync(this.User);

                var model = this.ordersService.GetOrderInputModel(user);
                if (model.OrderPrice < GlobalConstants.OrderPriceMin)
                {
                    return this.RedirectPermanent("/Administration/Products");
                }

                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User?.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        // Post Orders/Create
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(OrderInpitModel model)
        {
            if (DateTime.Now.TimeOfDay > GlobalConstants.CloseTime || DateTime.Now.TimeOfDay < GlobalConstants.OpenTime)
            {
                return this.Redirect("/");
            }

            if (!this.ModelState.IsValid)
            {
                foreach (var item in this.ModelState)
                {
                    Console.WriteLine(item.Key.ToString());
                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                return this.RedirectToAction("Create");
            }

            try
            {
                var user = await this.userManager.GetUserAsync(this.User);
                var id = await this.ordersService.CreateAsync(user, model);
                await this.hubAdmin.Clients.All.SendAsync("OperatorNewOrder", id);
                return this.Redirect("/Orders/UserOrders");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User?.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        // Ajax Orders/Details
        [Authorize(Roles = "Operator")]
        public IActionResult Details(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return this.NotFound();
            }

            bool result = int.TryParse(orderId, out int id);
            if (!result || !this.ordersService.IsExists(id))
            {
                return this.NotFound();
            }

            try
            {
                var model = this.ordersService.GetDetailsById(id);
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User?.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        // Orders/All
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> All()
        {
            var model = await this.ordersService.GetAllAsync();
            return this.View(model);
        }

        // Orders/All => OrdersByUser
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> OrdersByUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return this.NotFound();
            }

            try
            {
                this.ViewData["Title"] = userName;
                var model = await this.ordersService.GetOrdersByUserNameAsync(userName);
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User?.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        // Orders/UserOrders
        public async Task<IActionResult> UserOrders()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.Redirect(GlobalConstants.LoginPageRoute);
            }

            try
            {
                var model = await this.ordersService.GetMyOrdersAsync(user);
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User?.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        // Orders/UserOrders/UserOrderDetails
        public IActionResult UserOrderDetails(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return this.NotFound();
            }

            bool result = int.TryParse(orderId, out int id);
            if (!result || !this.ordersService.IsExists(id))
            {
                return this.NotFound();
            }

            try
            {
                var model = this.ordersService.GetUserDetailsById(id);
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User?.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        // Ajax Orders/OrdersByUser
        [Authorize(Roles =GlobalConstants.AdministratorName)]
        public async Task<IActionResult> ChangeStatus(string orderId, string status)
        {
            try
            {
                var userId = await this.ordersService.ChangeStatusAsync(status, orderId, string.Empty, string.Empty);
                this.hubUser.Clients.User(userId)?.SendAsync("UserStatusChanged", orderId, status);
                return this.Ok();
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User?.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }
    }
}
