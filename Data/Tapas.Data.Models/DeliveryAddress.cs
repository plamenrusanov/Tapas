namespace Tapas.Data.Models
{
    using System;
    using System.Text;

    using Tapas.Data.Common.Models;

    public class DeliveryAddress : BaseDeletableModel<string>
    {
        public DeliveryAddress()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string DisplayName { get; set; }

        public string City { get; set; }

        public string Borough { get; set; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public string Block { get; set; }

        public string Entry { get; set; }

        public string Floor { get; set; }

        public string AddInfo { get; set; }

        public string ApplicationUserId { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.City}, кв.{this.Borough}, ул.{this.Street} {this.StreetNumber}, бл.{this.Block}, вх.{this.Entry}, ет.{this.Floor}");
            return sb.ToString();
        }
    }
}
