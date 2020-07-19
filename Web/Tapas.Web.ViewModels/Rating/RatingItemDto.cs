namespace Tapas.Web.ViewModels.Rating
{
    using Newtonsoft.Json;

    public class RatingItemDto
    {
        [JsonProperty(propertyName: "itemId")]
        public string ItemId { get; set; }

        [JsonProperty(propertyName: "rating")]
        public string Rating { get; set; }
    }
}
