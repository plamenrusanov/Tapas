namespace Tapas.Web.ViewModels.Orders
{
    using System;

    using Tapas.Data.Models.Enums;

    public class OrderCollectionViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public DateTime DateTime { get; set; }

        public OrderStatus Status { get; set; }

        public decimal Rating { get; set; }

        public string Comment { get; set; }
    }
}
