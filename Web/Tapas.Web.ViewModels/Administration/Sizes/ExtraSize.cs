﻿namespace Tapas.Web.ViewModels.Administration.Sizes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Tapas.Web.ViewModels.Administration.Packages;

    public class ExtraSize
    {
        public ExtraSize()
        {
            this.AvailablePackages = new List<PackageViewModel>();
        }

        public int Index { get; set; }

        public int SizeId { get; set; }

        [Required]
        [MinLength(3)]
        public string SizeName { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999.99")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 5000)]
        public int Weight { get; set; }

        [Required]
        [Range(1, 100)]
        public int MaxProductsInPackage { get; set; }

        [Required]
        public int? PackageId { get; set; }

        public List<PackageViewModel> AvailablePackages { get; set; }

        [Required]
        public string MistralName { get; set; }

        [Required]
        public int MistralCode { get; set; }
    }
}
