﻿namespace Tapas.Web.Areas.Administration.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Services.Contracts;
    using Tapas.Services.Dto.Mistral;

    public class MistralController : AdministrationController
    {
        private readonly IMistralService mistralService;

        public MistralController(IMistralService mistralService)
        {
            this.mistralService = mistralService;
        }

        public async Task<ICollection<ProductMDto>> GetMProduct(string name)
        {
            if (name != null)
            {
                name.ToUpper();
            }

            var products = await this.mistralService.GetAllData(1, name);

            return products;
        }
    }
}
