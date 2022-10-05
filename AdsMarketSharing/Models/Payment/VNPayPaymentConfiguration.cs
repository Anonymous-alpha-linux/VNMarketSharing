namespace AdsMarketSharing.Models.Payment
{
    public class VNPayPaymentConfiguration
    {
        //  "HashSecret": "", // Chuoi bi mat
        //"ReturnUrl": "", // URL nhan ket qua tra ve
        //"Url": "", // URL thanh toan cua VNPAY 
        //"TmnCode": "", 
        public string HashSecret { get; set; }
        public string ReturnUrl { get; set; }
        public string Url { get; set; }
        public string TmnCode { get; set; }
        public string APIReturn { get; set; }
    }
}
