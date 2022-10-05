using System.Text.Json.Serialization;

namespace AdsMarketSharing.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BankCode
    {
        NCB,
        AGRIBANK,
        SCB,
        SACOMBANK,
        EXIMBANK,
        MSBANK,
        NAMABANK,
        VNMART,
        VIETINBANK,
        VIETCOMBANK,
        HDBANK,
        DONGABANK,
        TPBANK,
        OJB,
        BIDV,
        TECHCOMBANK,
        VPBANK,
        MBBANK,
        ACB,
        OCB,
        IVB,
        VISA
    }
}
