﻿namespace Tapas.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
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
                    Еntry = x.Еntry,
                    Floor = x.Floor,
                    AddInfo = x.AddInfo,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                }).ToList();
        }
    }
}