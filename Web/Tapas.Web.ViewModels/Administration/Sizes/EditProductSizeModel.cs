namespace Tapas.Web.ViewModels.Administration.Sizes
{
    using System.ComponentModel.DataAnnotations;

    public class EditProductSizeModel
    {
        public int SizeId { get; set; }

        [Required]
        [MinLength(3)]
        public string SizeName { get; set; }

        [Required]
        [Range(typeof(decimal), minimum: "0.01", maximum: "999.99", ErrorMessage = "Цената може да е между 0.01 и 999.99")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 5000)]
        public int Weight { get; set; }

        [Required]
        [Range(1, 100)]
        public int MaxProductsInPackage { get; set; }

        [Required]
        public int? PackageId { get; set; }

        // [Required]
        public string MistralName { get; set; }

        // [Required]
        public int MistralCode { get; set; }
    }
}
