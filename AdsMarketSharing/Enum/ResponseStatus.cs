using System.Text.Json.Serialization;
namespace AdsMarketSharing.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponseStatus
    {
        Successed,
        Failed,
        ActivatedAccount,
        NoActivatedAccount,
        EnableAccount,
        NoEnableAccount,
        NotAcceptableToken,
    }
}
