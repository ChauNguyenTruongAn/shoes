using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace ShoesShop.API.Features.Payments
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["VnPay:ReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((long)model.Amount * 100).ToString()); // Số tiền phải nhân 100 theo chuẩn VNPay
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {model.OrderId}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu (phải Unique, dùng tick là hợp lý)

            var paymentUrl = pay.CreateRequestUrl(_configuration["VnPay:BaseUrl"], _configuration["VnPay:HashSecret"]);
            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    pay.AddResponseData(key, value.ToString());
                }
            }

            var orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef"));
            var vnPayMac = collections["vnp_SecureHash"];
            var vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = pay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = pay.ValidateSignature(vnPayMac, _configuration["VnPay:HashSecret"]);

            if (!checkSignature)
            {
                return new PaymentResponseModel { Success = false, VnPayResponseCode = "INVALID_SIGNATURE" };
            }

            return new PaymentResponseModel
            {
                Success = vnp_ResponseCode == "00", // "00" là mã thành công của VNPay
                PaymentMethod = "VNPAY",
                OrderDescription = vnp_OrderInfo,
                OrderId = orderId.ToString(),
                TransactionId = pay.GetResponseData("vnp_TransactionNo"),
                VnPayResponseCode = vnp_ResponseCode
            };
        }
    }
}