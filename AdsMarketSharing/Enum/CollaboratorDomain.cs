using System.Text.Json.Serialization;
namespace AdsMarketSharing.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CollaboratorDomain
    {
        ITMarketing,
        SocialMarketing,
        Blogger,
        Other
    }
}
