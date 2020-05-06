﻿namespace Tapas.Web.ViewModels.Administration.Sizes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EditProductSizeModel
    {
        public int SizeId { get; set; }

        [Required]
        [MinLength(3)]
        public string SizeName { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Price { get; set; }

        [Required]
        [Range(10, 5000)]
        public int Weight { get; set; }

        [Required]
        [Range(1, 100)]
        public int MaxProductsInPackage { get; set; }

        [Required]
        public int? PackageId { get; set; }
    }
}
