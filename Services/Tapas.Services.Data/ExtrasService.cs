namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Tapas.Common;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Extras;

    public class ExtrasService : IExtrasService
    {
        private readonly IDeletableEntityRepository<Extra> extrasRepository;

        public ExtrasService(IDeletableEntityRepository<Extra> extrasRepository)
        {
            this.extrasRepository = extrasRepository;
        }

        public async Task Activate(int id)
        {
            var extras = this.CheckExistsReturnExtras(id);
            this.extrasRepository.Undelete(extras);
            await this.extrasRepository.SaveChangesAsync();
        }

        public IEnumerable<ExtraCartItemModel> All(bool isDeleted)
        {
            return this.extrasRepository
                .AllWithDeleted()
                .Where(x => x.IsDeleted == isDeleted)
                .Select(x => new ExtraCartItemModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Weight = x.Weight,
                }).ToList();
        }

        public Create Create()
        {
            return new Create();
        }

        public async Task Create(Create model)
        {
            var extra = new Extra()
            {
                Name = model.Name,
                Price = model.Price,
                Weight = model.Weight,
                MistralCode = model.MistralCode,
                MistralName = model.MistralName,
            };

            await this.extrasRepository.AddAsync(extra);
            await this.extrasRepository.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var extras = this.CheckExistsReturnExtras(id);
            this.extrasRepository.Delete(extras);
            await this.extrasRepository.SaveChangesAsync();
        }

        public EditModel Edit(int id)
        {
            var extras = this.CheckExistsReturnExtras(id);

            return new EditModel()
            {
                Id = extras.Id,
                Name = extras.Name,
                Price = extras.Price,
                Weight = extras.Weight,
                MistralCode = extras.MistralCode,
                MistralName = extras.MistralName,
            };
        }

        public async Task Edit(EditModel model)
        {
            var extra = this.CheckExistsReturnExtras(model.Id);
            extra.Name = model.Name;
            extra.Price = model.Price;
            extra.Weight = model.Weight;
            extra.MistralCode = model.MistralCode;
            extra.MistralName = model.MistralName;
            this.extrasRepository.Update(extra);
            await this.extrasRepository.SaveChangesAsync();
        }

        private Extra CheckExistsReturnExtras(int id)
        {
            var extras = this.extrasRepository
                .AllWithDeleted()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (extras is null)
            {
                throw new ArgumentException(string.Format(ExceptionMessages.NotExists, nameof(extras)));
            }

            return extras;
        }
    }
}
