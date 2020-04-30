﻿namespace Tapas.Web.ViewModels.Administration.Products
{
    using System.Collections.Generic;

    using Tapas.Web.ViewModels.Administration.Allergens;
    using Tapas.Web.ViewModels.Administration.Sizes;

    public class DetailsProductAddItemVM
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public List<DetailsAllergenViewModel> Allergens { get; set; }

        public List<ProductSizeViewModel> Sizes { get; set; }
    }
}
