namespace Tapas.Services.Dto.Geolocation
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class PositionDto
    {
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("licence")]
        public string Licence { get; set; }

        [JsonProperty("osm_type")]
        public string OsmType { get; set; }

        [JsonProperty("osm_id")]
        public string OsmId { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("lat")]
        public string Latitude { get; set; }

        [JsonProperty("lon")]
        public string Longitude { get; set; }

        [JsonProperty("address")]
        public AddressDto Address { get; set; }

        [JsonProperty("boundingbox")]
        public List<string> Boundingbox { get; set; }
    }
}
