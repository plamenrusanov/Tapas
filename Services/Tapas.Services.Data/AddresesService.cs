﻿namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Contracts;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Addreses;

    public class AddresesService : IAddresesService
    {
        private readonly IDeletableEntityRepository<DeliveryAddress> addressRepository;
        private readonly IGeolocationService geolocationService;

        public AddresesService(
            IDeletableEntityRepository<DeliveryAddress> addressRepository,
            IGeolocationService geolocationService)
        {
            this.addressRepository = addressRepository;
            this.geolocationService = geolocationService;
        }

        public async Task CreateAddressAsync(ApplicationUser user, AddressInputModel model)
        {
            var address = new DeliveryAddress()
            {
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                DisplayName = this.GetDisplayName(model),
                City = model.City,
                Borough = model.Borough,
                Street = model.Street,
                StreetNumber = model.StreetNumber,
                Block = model.Block,
                Entry = model.Entry,
                Floor = model.Floor,
                AddInfo = model.AddInfo,
                ApplicationUserId = user.Id,
            };

            await this.addressRepository.AddAsync(address);
            await this.addressRepository.SaveChangesAsync();
        }

        public async Task<AddressInputModel> GetAddressAsync(string latitude, string longitude)
        {
            var positionDto = await this.geolocationService.GetAddressAsync(latitude, longitude);

            return new AddressInputModel()
            {
                DisplayName = positionDto.display_name,
                Latitude = positionDto.lat,
                Longitude = positionDto.lon,
                City = positionDto.Address.city,
                Borough = positionDto.Address.suburb,
                Street = positionDto.Address.road,
                StreetNumber = positionDto.Address.house_number,
                Block = positionDto.Address.address29,
            };
        }

        public ICollection<AddressViewModel> GetMyAddreses(ApplicationUser user)
        {
            return this.addressRepository
                .All()
                .Where(x => x.ApplicationUserId == user.Id)
                .Select(x => new AddressViewModel()
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    City = x.City,
                    Borough = x.Borough,
                    Street = x.Street,
                    StreetNumber = x.StreetNumber,
                    Block = x.Block,
                    Еntry = x.Entry,
                    Floor = x.Floor,
                    AddInfo = x.AddInfo,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                }).ToList();
        }

        private string GetDisplayName(AddressInputModel model)
        {
            StringBuilder sb = new StringBuilder();
            if (model.Floor != null)
            {
                sb.Append($"ет. {model.Floor}, ");
            }

            if (model.Entry != null)
            {
                sb.Append($"вх. {model.Entry}, ");
            }

            if (model.Block != null)
            {
                sb.Append($"бл. {model.Block}, ");
            }

            if (model.Street != null)
            {
                sb.Append($"ул. {model.Street}, ");
            }

            if (model.StreetNumber != null)
            {
                sb.Append($"№ {model.StreetNumber}, ");
            }

            if (model.Borough != null)
            {
                sb.Append($"кв. {model.Borough}, ");
            }

            if (model.City != null)
            {
                sb.Append($"гр. {model.City}");
            }

            return sb.ToString();
        }
    }
}
