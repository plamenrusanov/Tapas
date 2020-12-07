namespace Tapas.Web.ViewModels.Administration.Categories
{
    using Newtonsoft.Json;
    using Tapas.Data.Models;
    using Tapas.Services.Mapping;

    public class CategoryViewModel : IMapTo<Category>, IMapFrom<Category>
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }
    }
}
