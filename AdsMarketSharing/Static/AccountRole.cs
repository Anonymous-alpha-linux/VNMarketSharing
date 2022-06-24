using System.Text.Json.Serialization;

namespace AdsMarketSharing.Static
{
    /*  [JsonConverter(typeof(JsonStringEnumConverter))]*/
    public static class AccountRole
    {
        public const string Administrator = "Administrator";
        public const string Collaborator = "Collaborator";
        public const string Marketter = "Marketter";
        public const string ServiceStaff = "ServiceStaff";
    }
}
