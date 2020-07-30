namespace Tapas.Services.Dto.Mistral
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OrderItemMDto
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public string Mea { get; set; }

        public decimal LastDeliveryPrice { get; set; }

        [Required]
        public decimal SalesPrice { get; set; }

        public decimal QttyOrder { get; set; }

        [Required]
        public decimal Qtty { get; set; }

        public decimal Discount { get; set; }

        public DateTime? DateEnd { get; set; }

        public string Lot { get; set; }

        public string Barcode { get; set; }
    }
}
