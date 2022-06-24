using System.Text.Json.Serialization;

namespace AdsMarketSharing.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountStatus
    {
        Newbie,
        Annual,
        Leaved
    }
}
