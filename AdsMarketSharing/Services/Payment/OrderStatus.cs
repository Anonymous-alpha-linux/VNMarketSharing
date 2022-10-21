using System.Text.Json.Serialization;

namespace AdsMarketSharing.Services.Payment
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Pending,
        Waiting,
        Delivering,
        Completed, 
        Cancelled
    }
}
