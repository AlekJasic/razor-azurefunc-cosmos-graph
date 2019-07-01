using Newtonsoft.Json;
using System;

namespace Blazor_CORE.Shared.Models
{
    public partial class ConcertMasters
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "concertNo")]
        public int ConcertNo { get; set; }
        [JsonProperty(PropertyName = "hallId")]
        public string HallId { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "concertDate")]
        public DateTime ConcertDate { get; set; }
        [JsonProperty(PropertyName = "ticketServiceName")]
        public string TicketServiceName { get; set; }
    }
}
