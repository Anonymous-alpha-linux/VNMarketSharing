using System.Text.Json.Serialization;

namespace AdsMarketSharing.Services.Payment
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Pending,
        Waiting,
        Delivering,
        Delivered,
        Completed, 
        Cancelled,
        CustomerNotReceived,
        SellerDenied
    }
    public class OrderScriptMessage
    {
        public static string PendingMessage = "Product are pending, please waiting";
        public static string WaitingMessage = "Product has being queue to wait for comfirmation from seller";
        public static string DeliveringMessage = "Product has been delivering to user, please charging the time and keep your dial to get item";
        public static string DeliveredMessage = "Product has come to you, please keep a call to receive item";
        public static string CompletedMessage = "User has received and checkout";
        public static string CancelledMessage = "Product has been canceled";
        public static string CustomerNotReceivedMessage = "Customer has denied the delivery";
        public static string SellerDeniedMessage = "Product has been canceled from seller";
    }
}
