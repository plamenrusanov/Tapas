namespace Tapas.Web.ViewModels.Extras
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Create
    {
        [RequiredBg]
        [StringLength(30, MinimumLength = 3)]
        [Display(Name = "Име")]
        public string Name { get; set; }

        [RequiredBg]
        [Range(typeof(decimal), minimum: "0.01", maximum: "999.99")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [RequiredBg]
        [Range(minimum: 1, maximum: 1000)]
        [Display(Name = "Тегло  гр.")]
        public int Weight { get; set; }

        [RequiredBg]
        public int MistralCode { get; set; }

        [RequiredBg]
        public string MistralName { get; set; }
    }
}
