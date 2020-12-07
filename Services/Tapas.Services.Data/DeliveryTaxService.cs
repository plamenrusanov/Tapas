namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.DeliveryTax;

    public class DeliveryTaxService : IDeliveryTaxService
    {
        private readonly IDeletableEntityRepository<DeliveryTax> deliveryTaxRepository;

        public DeliveryTaxService(
            IDeletableEntityRepository<DeliveryTax> deliveryTaxRepository)
        {
            this.deliveryTaxRepository = deliveryTaxRepository;
        }

        public async Task AddAsync(DeliveryTaxInpitModel inputModel)
        {
            await this.deliveryTaxRepository.AddAsync(
                new DeliveryTax()
                {
                    Price = inputModel.Price,
                    MistralCode = inputModel.MistralCode,
                    MistralName = inputModel.MistralName,
                });
            await this.deliveryTaxRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<DeliveryTaxViewModel>> AllAsync(bool isDeleted)
        {
            return await this.deliveryTaxRepository
                .AllWithDeleted()
                .Where(x => x.IsDeleted == isDeleted)
                .Select(x => new DeliveryTaxViewModel()
                {
                    Id = x.Id,
                    Price = x.Price,
                    MistralCode = x.MistralCode,
                    MistralName = x.MistralName,
                }).ToListAsync();
        }

        public async Task EditAsync(DeliveryTaxViewModel viewModel)
        {
            var tax = this.ExistTaxById(viewModel.Id);
            tax.Price = viewModel.Price;
            tax.MistralCode = viewModel.MistralCode;
            tax.MistralName = viewModel.MistralName;
            this.deliveryTaxRepository.Update(tax);
            await this.deliveryTaxRepository.SaveChangesAsync();
        }

        public DeliveryTaxViewModel GetDeliveryTaxViewModelById(int deliveryTaxId)
        {
            var tax = this.ExistTaxById(deliveryTaxId);

            return new DeliveryTaxViewModel()
            {
                Id = tax.Id,
                Price = tax.Price,
                MistralCode = tax.MistralCode,
                MistralName = tax.MistralName,
            };
        }

        public async Task RemoveAsync(int deliveryTaxId)
        {
            var tax = this.ExistTaxById(deliveryTaxId);

            this.deliveryTaxRepository.Delete(tax);

            await this.deliveryTaxRepository.SaveChangesAsync();
        }

        public async Task ActivateAsync(int deliveryTaxId)
        {
            var tax = this.ExistTaxById(deliveryTaxId);

            this.deliveryTaxRepository.Undelete(tax);

            await this.deliveryTaxRepository.SaveChangesAsync();
        }

        private DeliveryTax ExistTaxById(int deliveryTaxId)
        {
            var tax = this.deliveryTaxRepository.AllWithDeleted().FirstOrDefault(x => x.Id == deliveryTaxId);

            if (tax is null)
            {
                throw new ArgumentException();
            }

            return tax;
        }
    }
}
