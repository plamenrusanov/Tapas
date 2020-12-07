namespace Tapas.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Tapas.Data.Common.Models;

    public class Category : BaseDeletableModel<string>
    {
        public Category()
            : base()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Products = new HashSet<Product>();
        }

        public string Name { get; set; }

        public int Position { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
