namespace Tapas.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Tapas.Common;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Extras;

    [Authorize]
    public class ExtrasController : BaseController
    {
        private readonly IExtrasService extrasService;
        private readonly ILogger<ExtrasController> logger;

        public ExtrasController(
            IExtrasService extrasService,
            ILogger<ExtrasController> logger)
        {
            this.extrasService = extrasService;
            this.logger = logger;
        }

        public IActionResult Index(bool isDeleted = false)
        {
            try
            {
                var model = this.extrasService.All(isDeleted);
                this.ViewData["isDeleted"] = isDeleted;
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [Authorize(Roles = GlobalConstants.AdministratorName)]
        public IActionResult GetCreateModel()
        {
            try
            {
                var model = this.extrasService.Create();
                return this.View();
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [Authorize(Roles = GlobalConstants.AdministratorName)]
        public async Task<IActionResult> Create(Create model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewName: "GetCreateModel");
            }

            try
            {
                await this.extrasService.Create(model);
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [Authorize(Roles = GlobalConstants.AdministratorName)]
        public IActionResult GetEditModel(int id)
        {
            try
            {
                var model = this.extrasService.Edit(id);
                return this.View(model);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [Authorize(Roles = GlobalConstants.AdministratorName)]
        public async Task<IActionResult> Edit(EditModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewName: "GetEditModel", model: model);
            }

            try
            {
                await this.extrasService.Edit(model);
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [Authorize(Roles = GlobalConstants.AdministratorName)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await this.extrasService.Delete(id);
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }

        [Authorize(Roles = GlobalConstants.AdministratorName)]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                await this.extrasService.Activate(id);
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.BadRequest();
            }
        }
    }
}
