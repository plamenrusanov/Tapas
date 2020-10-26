namespace Tapas.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Tapas.Common;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.DeliveryTax;

    public class DeliveryTaxController : AdministrationController
    {
        private readonly IDeliveryTaxService deliveryTaxService;
        private readonly ILogger<DeliveryTaxController> logger;

        public DeliveryTaxController(
            IDeliveryTaxService deliveryTaxService,
            ILogger<DeliveryTaxController> logger)
        {
            this.deliveryTaxService = deliveryTaxService;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(bool isDeleted = false)
        {
            if (this.User == null)
            {
                return this.RedirectToPage("/Account/Login");
            }

            try
            {
                var taxes = await this.deliveryTaxService.AllAsync(isDeleted);
                this.ViewData["isDeleted"] = isDeleted;
                return this.View(taxes);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(DeliveryTaxInpitModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            try
            {
                await this.deliveryTaxService.AddAsync(inputModel);

                return this.RedirectToAction("Index");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        public IActionResult Edit(int deliveryTaxId)
        {
            try
            {
                var tax = this.deliveryTaxService.GetDeliveryTaxViewModelById(deliveryTaxId);
                return this.View(tax);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DeliveryTaxViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            try
            {
                await this.deliveryTaxService.EditAsync(viewModel);
                return this.RedirectToAction("Index");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        public async Task<IActionResult> Delete(int deliveryTaxId)
        {
            try
            {
                await this.deliveryTaxService.RemoveAsync(deliveryTaxId);

                return this.RedirectToAction("Index");
            }
            catch (Exception e)
            {
                this.logger.LogInformation(GlobalConstants.DefaultLogPattern, this.User.Identity.Name, e.Message, e.StackTrace);
                return this.NotFound();
            }
        }

        public async Task<IActionResult> Activate(int deliveryTaxId)
        {
            try
            {
                await this.deliveryTaxService.ActivateAsync(deliveryTaxId);
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
