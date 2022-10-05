using System.Text.Json.Serialization;

namespace AdsMarketSharing.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderType
    {
        TOPUP,
        BILLPAYMENT,
        FASHION
    }
}
