﻿namespace Tapas.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Tapas.Common;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.Hubs;
    using Tapas.Web.Log;
    using Tapas.Web.ViewModels.Orders;

    public class OrdersController : Controller
    {
        private const int ORDERSCOUNT = 30;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOrdersService ordersService;
        private readonly IHubContext<OrderHub> hubAdmin;
        private readonly IHubContext<UserOrdersHub> hubUser;
        private readonly Logger logger;

        public OrdersController(
            UserManager<ApplicationUser> userManager,
            IOrdersService ordersService,
            IHubContext<OrderHub> hubAdmin,
            IHubContext<UserOrdersHub> hubUser)
        {
            this.userManager = userManager;
            this.ordersService = ordersService;
            this.hubAdmin = hubAdmin;
            this.hubUser = hubUser;
            this.logger = new Logger();
        }

        // Orders/Index
        [Authorize(Roles = GlobalConstants.OperatorName)]
        public IActionResult Index()
        {
            var orders = this.ordersService.GetDailyOrders();
            return this.View(orders);
        }

        // Post Orders/Create
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(OrderInpitModel model)
        {
            if (DateTime.Now.TimeOfDay > GlobalConstants.CloseTime || DateTime.Now.TimeOfDay < GlobalConstants.OpenTime)
            {
                return this.Redirect("/");
            }

            if (!this.ModelState.IsValid)
            {
                return this.Redirect("ShoppingCart/Index");
            }

            try
            {
                ApplicationUser user = await this.GetUser(model);

                var id = await this.ordersService.CreateAsync(user, model);
                await this.hubAdmin.Clients.All.SendAsync("OperatorNewOrder", id);
                this.TempData["NewOrder"] = true;
                return this.Ok();
            }
            catch (Exception e)
            {
                await this.logger.WriteException(e);
                await this.logger.WriteObject(typeof(OrderInpitModel), model);
                return this.BadRequest();
            }
        }

        // Ajax Orders/Details
        [Authorize(Roles = GlobalConstants.OperatorName)]
        public async Task<IActionResult> Details(string orderId)
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
               // await this.logger.WriteMessage($"Име: {this.User.Identity.Name} || От: {this.Request.HttpContext.Connection.RemoteIpAddress} || Видяна: {DateTime.Now:dd/MM/yy HH:mm} || Създадена: {model.CreatedOn}");
                return this.View(model);
            }
            catch (Exception e)
            {
                await this.logger.WriteException(e);
                return this.NotFound();
            }
        }

        // Orders/All
        [Authorize(Roles = GlobalConstants.AdministratorName)]
        public async Task<IActionResult> All()
        {
            var model = await this.ordersService.GetAllAsync(0, ORDERSCOUNT);
            return this.View(model);
        }

        // Orders/All => OrdersByUser
        [Authorize(Roles = GlobalConstants.AdministratorName)]
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
                await this.logger.WriteException(e);
                return this.BadRequest();
            }
        }

        // Orders/UserOrders
        [AllowAnonymous]
        public async Task<IActionResult> UserOrders()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                if (this.Request.Cookies.ContainsKey(GlobalConstants.UserIdCookieKey))
                {
                    user = await this.userManager.FindByIdAsync(this.Request.Cookies[GlobalConstants.UserIdCookieKey]);
                }
            }

            try
            {
                var model = await this.ordersService.GetMyOrdersAsync(user);
                return this.View(model);
            }
            catch (Exception e)
            {
                await this.logger.WriteException(e);
                return this.NotFound();
            }
        }

        // Orders/UserOrders/UserOrderDetails
        [AllowAnonymous]
        public async Task<IActionResult> UserOrderDetails(string orderId)
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
                await this.logger.WriteException(e);
                return this.NotFound();
            }
        }

        // Ajax Orders/OrdersByUser
        [Authorize(Roles =GlobalConstants.AdministratorName)]
        public async Task<IActionResult> ChangeStatus(string orderId, string status, string setTime, string deliveryFee)
        {
            try
            {
                var userId = await this.ordersService.ChangeStatusAsync(status, orderId, setTime, deliveryFee);
                this.hubUser.Clients.User(userId)?.SendAsync("UserStatusChanged", orderId, status);
                return this.Ok();
            }
            catch (Exception e)
            {
                await this.logger.WriteException(e);
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadOrders(int skip = 0)
        {
            var model = await this.ordersService.GetAllAsync(skip, ORDERSCOUNT);

            if (!model.Any())
            {
                return this.StatusCode(204);  // 204 := "No Content"
            }

            return this.PartialView("AllPartial", model);
        }

        private async Task<ApplicationUser> GetUser(OrderInpitModel model)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null && this.Request.Cookies.ContainsKey(GlobalConstants.UserIdCookieKey))
            {
                user = await this.userManager.FindByIdAsync(this.Request.Cookies[GlobalConstants.UserIdCookieKey]);
            }

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = model.Username,
                    PhoneNumber = model.Phone,
                    Email = $"{this.Request.HttpContext.Connection.RemoteIpAddress}@tapas.bg{DateTime.Now}",
                };
                await this.userManager.CreateAsync(user);
                this.Response.Cookies.Append(GlobalConstants.UserIdCookieKey, user.Id, new CookieOptions()
                {
                    Domain = this.Request.Host.Host,
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddYears(2),
                    Path = GlobalConstants.IndexRoute,
                });
            }

            return user;
        }
    }
}
