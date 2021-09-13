namespace Tapas.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Tapas.Common;
    using Tapas.Data;

    public class DashboardController : AdministrationController
    {
        private readonly ApplicationDbContext db;

        public DashboardController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public JsonResult OpenClose(bool isOpen)
        {
            GlobalConstants.IsOpen = isOpen;

            var str = isOpen ? "отворихте!" : "затворихте!";

            try
            {
                var claim = this.db.Users.Find("229c10f5-1596-4300-aa09-17c84cb44e56").Claims.First();

                claim.ClaimValue = isOpen.ToString();

                this.db.SaveChanges();
            }
            catch (Exception e)
            {
                return this.Json(e.Message);
            }

            return this.Json($"Успешно {str}");
        }
    }
}
