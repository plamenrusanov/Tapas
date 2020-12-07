namespace Tapas.Web.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Tapas.Services.Messaging;
    using Tapas.Web.ViewModels;

    public class HomeController : BaseController
    {
        private readonly ITwilioSmsSenderService twilioSmsSender;

        public HomeController(ITwilioSmsSenderService twilioSmsSender)
        {
            this.twilioSmsSender = twilioSmsSender;
        }

        public async Task<IActionResult> Index()
        {
            // await this.twilioSmsSender.SendSms("+359890321780", 123456);
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
