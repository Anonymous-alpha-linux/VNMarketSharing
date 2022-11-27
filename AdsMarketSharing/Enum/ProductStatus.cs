using System.Text.Json.Serialization;

namespace AdsMarketSharing.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductStatus
    {
        New,
        HasDenied,
        OutOfStorage,
        Expired
    }
}
