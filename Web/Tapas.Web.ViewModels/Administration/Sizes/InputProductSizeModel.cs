namespace Tapas.Web.ViewModels.Administration.Sizes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class InputProductSizeModel
    {
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Името може да съдържа между 3 и 30 символа!")]
        public string SizeName { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999.99")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 500000)]
        public int Weight { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]
        [Range(1, 100)]
        public int MaxProductsInPackage { get; set; }

        [Required]
        public string MistralName { get; set; }

        [Required]
        public int MistralCode { get; set; }
    }
}
