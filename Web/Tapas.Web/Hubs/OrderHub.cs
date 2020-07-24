namespace Tapas.Web.Hubs
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Tapas.Data.Models.Enums;
    using Tapas.Services.Data.Contracts;

    public class OrderHub : Hub
    {
        private readonly IOrdersService ordersService;
        private readonly IAlarm alarm;
        private readonly ILogger<OrderHub> logger;
        private readonly IHubContext<UserOrdersHub> hubUser;

        public OrderHub(IOrdersService ordersService, IAlarm alarm, ILogger<OrderHub> logger, IHubContext<UserOrdersHub> hubUser)
        {
            this.ordersService = ordersService;
            this.alarm = alarm;
            this.logger = logger;
            this.hubUser = hubUser;
        }

        public async Task OperatorChangeStatus(string status, string order, string setTime, string taxId)
        {
            if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(order))
            {
                await this.Clients.Caller.SendAsync("OperatorAlertMessage", "Липсват данни за статуса или номера на поръчката!");
                return;
            }

            try
            {
                var userId = await this.ordersService.ChangeStatusAsync(status, order, setTime, taxId);
                await this.Clients.All.SendAsync("OperatorStatusChanged", order, status);
                await this.hubUser.Clients.User(userId)?.SendAsync("UserStatusChanged", order, status);
                if (Enum.TryParse(typeof(OrderStatus), status, out object statusResult))
                {
                    if ((OrderStatus)statusResult == OrderStatus.Processed)
                    {
                        Action action = new Action(() => this.alarm.CreateAlarm(order, setTime));
                        var t = new Task(action);
                        t.Start();
                    }
                }
            }
            catch (System.Exception e)
            {
                this.logger.LogInformation(e.Message);
                await this.Clients.Caller.SendAsync("OperatorAlertMessage", e.Message);
            }
        }
    }
}
