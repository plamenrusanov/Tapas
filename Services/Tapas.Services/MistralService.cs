namespace Tapas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using RestSharp;
    using Tapas.Services.Contracts;
    using Tapas.Services.Dto.Mistral;

    public class MistralService : IMistralService
    {
        private const string Url = "http://77.78.55.66:8066";
        private const string UserName = "1";
        private readonly string password;

        public MistralService(string password)
        {
            this.password = password;
        }

        // api/GetAllData?locationid=1&search=nameOrId
        public async Task<ICollection<ProductMDto>> GetAllData(int locationId, string search = null)
        {
            var client = new RestClient($"{Url}/api/GetAllData")
            {
                Timeout = -1,
            };

            try
            {
                var request = await this.GetRequestAsync();
                request.AddParameter("locationId", locationId);
                request.AddParameter("search", search);
                IRestResponse response = await client.ExecuteGetAsync(request);
                if (response.IsSuccessful)
                {
                    var dto = JsonConvert.DeserializeObject<ICollection<ProductMDto>>(response.Content);
                    return dto;
                }

                throw new Exception("Request failed!");
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    throw new Exception(e.InnerException.Message);
                }

                throw new Exception(e.Message);
            }
        }

        // api/Locations
        public async Task<ICollection<LocationMDto>> Locations()
        {
            var client = new RestClient($"{Url}/api/Locations")
            {
                Timeout = -1,
            };
            try
            {
                var request = await this.GetRequestAsync();
                IRestResponse response = await client.ExecuteGetAsync(request);
                if (response.IsSuccessful)
                {
                    return JsonConvert.DeserializeObject<ICollection<LocationMDto>>(response.Content);
                }

                throw new Exception("Request failed!");
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    throw new Exception(e.InnerException.Message);
                }

                throw new Exception(e.Message);
            }
        }

        // POST api/SaveWebOrder
        public async Task SaveWebOrder(OrderMDto order)
        {
            var client = new RestClient($"{Url}/api/SaveWebOrder")
            {
                Timeout = -1,
            };
            try
            {
                var request = await this.GetRequestAsync();
                request.AddJsonBody(JsonConvert.SerializeObject(order));
                IRestResponse response = await client.ExecutePostAsync(request);

                if (!response.IsSuccessful)
                {
                    throw new Exception("Request failed!");
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    throw new Exception(e.InnerException.Message);
                }

                throw new Exception(e.Message);
            }
        }

        // api/Storages
        public async Task<ICollection<StorageMDto>> Storages()
        {
            var client = new RestClient($"{Url}/api/Storages")
            {
                Timeout = -1,
            };
            try
            {
                var request = await this.GetRequestAsync();
                IRestResponse response = await client.ExecuteGetAsync(request);
                if (response.IsSuccessful)
                {
                    return JsonConvert.DeserializeObject<ICollection<StorageMDto>>(response.Content);
                }

                throw new Exception("Request failed!");
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    throw new Exception(e.InnerException.Message);
                }

                throw new Exception(e.Message);
            }
        }

        private async Task<RestRequest> GetRequestAsync()
        {
            TokenMDto tokenDto;
            try
            {
                tokenDto = await this.GetTokenAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {tokenDto.AccessToken}");
            return request;
        }

        private async Task<TokenMDto> GetTokenAsync()
        {
            var client = new RestClient($"{Url}/token")
            {
                Timeout = -1,
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", UserName);
            request.AddParameter("password", this.password);
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var dto = JsonConvert.DeserializeObject<TokenMDto>(response.Content);
                return dto;
            }

            throw new Exception(message: "Authorization failed!");
        }
    }
}
