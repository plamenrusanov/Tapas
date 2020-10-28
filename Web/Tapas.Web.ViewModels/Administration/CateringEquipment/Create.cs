namespace Tapas.Web.ViewModels.Administration.CateringEquipment
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class Create
    {
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Името трябва да е между 3 и 30 символа.")]
        public string Name { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        [Required]
        [StringLength(300)]
        public string Description { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999.99")]
        public decimal Price { get; set; }
    }
}
