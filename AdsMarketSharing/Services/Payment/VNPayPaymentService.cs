using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models;
using System.Threading.Tasks;
using AdsMarketSharing.Models.Payment;
using Microsoft.Extensions.Options;
using AdsMarketSharing.Entities;
using AdsMarketSharing.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using AdsMarketSharing.DTOs.Payment;

namespace AdsMarketSharing.Services.Payment
{
    public class VNPayPaymentService : IPaymentService
    {
        private readonly VNPayPaymentConfiguration ConfigurationManager;
        private IHttpContextAccessor _httpContextAccessor;

        public VNPayPaymentService(IOptions<VNPayPaymentConfiguration> vpnPayConfig, IHttpContextAccessor httpContextAccessor)
        {
            ConfigurationManager = vpnPayConfig.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<string>> Checkout(Invoice invoice, string returnUrl = null)
        {
            var response = new ServiceResponse<string>() { 
                Data = null,
                Message = "Completed checkout",
                ServerMessage = "Completed vnpay service",
                Status = Enum.ResponseStatus.Successed,
                StatusCode = 201
            };

            //Get Config Info
            string vnp_Returnurl = !String.IsNullOrEmpty(returnUrl) ? returnUrl : ConfigurationManager.ReturnUrl; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.Url; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.TmnCode; //Ma website
            string vnp_HashSecret = ConfigurationManager.HashSecret; //Chuoi bi mat
            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                response.Data = null;
                response.ServerMessage = "Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong file web.config";
                response.Status = Enum.ResponseStatus.Failed;
                response.Message = "Failed";
                response.StatusCode = 200;
                return response;
            }
            //Get payment input
            string locale = "vn";
            //Build URL for VNPAY
            VNPayLibrary vnpay = new VNPayLibrary();
            string host = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}";
            var hasOrigin = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Origin", out var origin);
            if (hasOrigin)
            {
                host = origin;
            }

            vnpay.AddRequestData("vnp_Version", VNPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (invoice.CashAmount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", invoice.Payment.BankCode.ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());
            vnpay.AddRequestData("vnp_Locale", "vn");
            string orderInfoString = String.Join("\n", invoice.Orders.Select(order => order.ToString()));
            vnpay.AddRequestData("vnp_OrderInfo", "Paid :");
            //vnpay.AddRequestData("vnp_OrderType", invoice.Payment.ToString()); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", host + vnp_Returnurl);
            string uniqueRef = $"{Guid.NewGuid().ToString()}-{Util.RandomString(26)}";
            vnpay.AddRequestData("vnp_TxnRef", uniqueRef); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            invoice.OnlineRef = uniqueRef;
            //Add Params of 2.1.0 Version
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddHours(1).ToString("yyyyMMddHHmmss"));

            ////Billing
            //vnpay.AddRequestData("vnp_Bill_Mobile", invoice..Trim());
            //vnpay.AddRequestData("vnp_Bill_Email", invoice.Buyer.Account.Email.Trim());
    
            //if (!String.IsNullOrEmpty(orderInformation.BuyerFullName))
            //{
            //    var indexof = orderInformation.BuyerFullName.IndexOf(' ');
            //    vnpay.AddRequestData("vnp_Bill_FirstName", orderInformation.BuyerFullName.Substring(0, indexof));
            //    vnpay.AddRequestData("vnp_Bill_LastName", orderInformation.BuyerFullName.Substring(indexof + 1, orderInformation.BuyerFullName.Length - indexof - 1));
            //}
            //vnpay.AddRequestData("vnp_Bill_Address", orderInformation.Address.StreetAddress.Trim());
            //vnpay.AddRequestData("vnp_Bill_City", orderInformation.Address.City.Trim());
            //vnpay.AddRequestData("vnp_Bill_Country", orderInformation.Address.Country.Trim());
            //vnpay.AddRequestData("vnp_Bill_State", "");

            //// Invoice
            //vnpay.AddRequestData("vnp_Inv_Phone", orderInformation.Address.PhoneNumber.Trim());
            //vnpay.AddRequestData("vnp_Inv_Email", orderInformation.Buyer.Account.Email.Trim());
            //vnpay.AddRequestData("vnp_Inv_Customer", orderInformation.Buyer.OrganizationName.Trim());
            //vnpay.AddRequestData("vnp_Inv_Type", orderInformation.Type.ToString());
            //vnpay.AddRequestData("vnp_Inv_Company", orderInformation.Merchant.Name.Trim());
            //vnpay.AddRequestData("vnp_Inv_Address", orderInformation.Address.ReceiverName);
            //vnpay.AddRequestData("vnp_Inv_Taxcode", orderInformation);

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            response.Data = paymentUrl;
            return response;    
        }

        public async Task<ServiceResponse<PaymentConfirmationDTO>> Confirm()
        {
            var response = new ServiceResponse<PaymentConfirmationDTO>() {
                Data = new PaymentConfirmationDTO(),
                Message = "",
                ServerMessage = "",
                Status = Enum.ResponseStatus.Successed,
                StatusCode = 200
            };
            var requestPayload = _httpContextAccessor.HttpContext.Request.Query;

            if (requestPayload.Count > 0)
            {
                string hashSecret = ConfigurationManager.HashSecret; //Chuỗi bí mật
                //var vnpayData = _httpContextAccessor.HttpContext.Request.QueryString;
                VNPayLibrary pay = new VNPayLibrary();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in requestPayload.Keys)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, requestPayload[s]);
                    }
                }

                string onlineRef = pay.GetResponseData("vnp_TxnRef"); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = pay.GetResponseData("vnp_SecureHash"); //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        response.StatusCode = 200;
                        response.Status = Enum.ResponseStatus.Successed;
                        response.Message = "Thanh toán thành công hóa đơn " + onlineRef + " | Mã giao dịch: " + vnpayTranId;
                        response.Data.OnlineRef = onlineRef;
                        response.Data.TransNo = vnpayTranId;
                        
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        response.StatusCode = 200;
                        response.Status = Enum.ResponseStatus.Failed;
                        response.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + onlineRef + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        response.Data.OnlineRef = onlineRef;
                    }
                    response.Data.StatusCode = vnp_ResponseCode;
                }
                else
                {
                    response.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return response;
        }
    }
}
