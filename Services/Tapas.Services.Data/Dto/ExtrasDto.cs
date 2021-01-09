namespace Tapas.Services.Data.Dto
{
    using Newtonsoft.Json;

    public class ExtrasDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("qty")]
        public int Quantity { get; set; }
    }
}
