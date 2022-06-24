using AdsMarketSharing.Enum;

namespace AdsMarketSharing.Models
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public ResponseStatus Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ServerMessage { get; set; }
    }
}
