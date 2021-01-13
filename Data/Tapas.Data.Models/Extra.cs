namespace Tapas.Data.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Tapas.Data.Common.Models;

    public class Extra : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Price { get; set; }

        public int Weight { get; set; }

        // [Required]
        public int MistralCode { get; set; }

        // [Required]
        public string MistralName { get; set; }

        public virtual ICollection<ExtraItem> ExtraItems { get; set; }
    }
}
