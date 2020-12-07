namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Tapas.Common;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Contracts;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.CateringEquipment;

    public class CateringEquipmentService : ICateringEquipmentService
    {
        private readonly IDeletableEntityRepository<EquipmentForRent> equipmentRepository;
        private readonly ICloudService cloudService;

        public CateringEquipmentService(
            IDeletableEntityRepository<EquipmentForRent> equipmentRepository,
            ICloudService cloudService)
        {
            this.equipmentRepository = equipmentRepository;
            this.cloudService = cloudService;
        }

        public async Task ActivateAsync(string id)
        {
            var equipment = this.CheckNullExistsReturnEntity(id);

            this.equipmentRepository.Undelete(equipment);
            await this.equipmentRepository.SaveChangesAsync();
        }

        public async Task AddEquipmentAsync(Create model)
        {
            var equipment = new EquipmentForRent()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
            };

            if (model.Image != null)
            {
                equipment.ImageUrl = await this.cloudService.UploadImageFromForm(model.Image);
            }

            await this.equipmentRepository.AddAsync(equipment);
            await this.equipmentRepository.SaveChangesAsync();
        }

        public Create CreateInputModel()
        {
            return new Create();
        }

        public async Task Delete(string id)
        {
            var equipment = this.CheckNullExistsReturnEntity(id);

            this.equipmentRepository.Delete(equipment);
            await this.equipmentRepository.SaveChangesAsync();
        }

        public IEnumerable<IndexEquipmentModel> GetAll()
        {
            var model = this.equipmentRepository
                 .All()
                 .Select(x => new IndexEquipmentModel()
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description,
                     Price = x.Price,
                     ImageUrl = x.ImageUrl,
                 })
                 .ToList();
            return model;
        }

        public IEnumerable<Deleted> GetDeletedProducts()
        {
            var model = this.equipmentRepository
                .AllWithDeleted()
                .Where(x => x.IsDeleted == true)
                .Select(x => new Deleted()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                })
                .ToList();
            return model;
        }

        public Details GetDetailsById(string id)
        {
            var equipment = this.CheckNullExistsReturnEntity(id);

            return new Details()
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Description = equipment.Description,
                Price = equipment.Price,
                ImageUrl = equipment.ImageUrl,
            };
        }

        public Edit GetEditModel(string id)
        {
            var equipment = this.CheckNullExistsReturnEntity(id);

            var model = new Edit()
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Description = equipment.Description,
                Price = equipment.Price,
                ImageUrl = equipment.ImageUrl,
            };

            return model;
        }

        public async Task SetEditModel(Edit model)
        {
            var equipment = this.CheckNullExistsReturnEntity(model.Id);

            equipment.Name = model.Name;
            equipment.Price = model.Price;
            equipment.Description = model.Description;

            if (model.Image != null)
            {
                equipment.ImageUrl = await this.cloudService.UploadImageFromForm(model.Image);
            }

            this.equipmentRepository.Update(equipment);
            await this.equipmentRepository.SaveChangesAsync();
        }

        private EquipmentForRent CheckNullExistsReturnEntity(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            var equipment = this.equipmentRepository
                .AllWithDeleted()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (equipment is null)
            {
                throw new Exception(string.Format(ExceptionMessages.NotExists, nameof(equipment)));
            }

            return equipment;
        }
    }
}
