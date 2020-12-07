namespace Tapas.Services.Data.Dto
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ShoppingCartItemDto
    {
        [JsonProperty("PId")]
        public string ProductId { get; set; }

        [JsonProperty("SId")]
        public int SizeId { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Qty")]
        public int Quantity { get; set; }

        [JsonProperty("Extras")]
        public List<ExtrasDto> Extras { get; set; }
    }
}
