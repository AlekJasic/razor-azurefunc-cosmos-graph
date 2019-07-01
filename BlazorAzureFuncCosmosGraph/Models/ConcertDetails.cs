using Newtonsoft.Json;

namespace Blazor_CORE.Shared.Models
{
    public partial class ConcertDetails
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "concertDetailNo")]
        public int ConcertDetailNo { get; set; }
        [JsonProperty(PropertyName = "concertNo")]
        public int ConcertNo { get; set; }
        [JsonProperty(PropertyName = "artistName")]
        public string ArtistName { get; set; }
        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }
        [JsonProperty(PropertyName = "price")]
        public int Price { get; set; }
    }
}
