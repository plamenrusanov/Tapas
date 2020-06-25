﻿namespace Tapas.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Tapas.Services.Contracts;
    using Tapas.Services.Dto.Geolocation;

    public class GeolocationService : IGeolocationService
    {
        private readonly string apiKey;

        public GeolocationService(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<PositionDto> GetAddressAsync(string latitude, string longitude)
        {
            if (string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
            {
                throw new ArgumentNullException();
            }

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip,
            };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("bg", 0.8));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                var dto = new PositionDto();
                try
                {
                    var result = await client.GetAsync($"https://eu1.locationiq.com/v1/reverse.php?key={this.apiKey}&lat={latitude}&lon={longitude}&format=json");
                    var resultContent = await result.Content.ReadAsStringAsync();
                    dto = JsonConvert.DeserializeObject<PositionDto>(resultContent);
                }
                catch (Exception)
                {
                    throw new Exception();
                }

                return dto;
            }
        }
    }
}
