namespace Tapas.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    using Tapas.Data.Common.Models;
    using Tapas.Data.Models.Enums;

    public class Order : BaseModel<int>
    {
        public Order()
        {
            this.Bag = new ShopingCart();
        }

        public string AddInfo { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string AddressId { get; set; }

        public virtual DeliveryAddress Address { get; set; }

        public string BagId { get; set; }

        public virtual ShopingCart Bag { get; set; }

        public OrderStatus Status { get; set; }

        public int MinutesForDelivery { get; set; }

        public DateTime ProcessingTime { get; set; }

        public DateTime OnDeliveryTime { get; set; }

        public DateTime DeliveredTime { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal DeliveryFee { get; set; }

        public int? DeliveryTaxId { get; set; }

        public virtual DeliveryTax DeliveryTax { get; set; }

        public bool TakeAway { get; set; }

        public int Cutlery { get; set; }
    }
}
