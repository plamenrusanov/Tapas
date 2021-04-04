namespace Tapas.Web.Controllers
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Tapas.Common;
    using Tapas.Web.Log;

    [Authorize(Roles = GlobalConstants.AdministratorName)]
    public class LogController : Controller
    {
        private readonly Logger logger;

        public LogController()
        {
            this.logger = new Logger();
        }

        public IActionResult Index()
        {
            this.ViewData["sizeOfFile"] = this.logger.GetSizeOfLogFile();
            List<string> log = this.logger.ReadLog();
            return this.View(log);
        }

    }
}
