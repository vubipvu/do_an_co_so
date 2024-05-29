using System;
using System.Net.Http;
using System.Text;
using Baith.Payment.Helpers;
using BaiTh.Payment.Momo.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BaiTh.Payment.Momo.Request
{
    public class MomoOneTimePaymentRequest
    {
        public MomoOneTimePaymentRequest(string partnerCode, string requestId,
            long amount, string orderId, string orderInfo, string redirectUrl,
            string ipnUrl, string requestType, string extraData, string lang = "vi")
        {
            PartnerCode = partnerCode;
            RequestId = requestId;
            Amount = amount;
            OrderId = orderId;
            OrderInfo = orderInfo;
            RedirectUrl = redirectUrl;
            IpnUrl = ipnUrl;
            RequestType = requestType;
            ExtraData = extraData;
            Lang = lang;
        }

        public string PartnerCode { get; }
        public string RequestId { get; }
        public long Amount { get; }
        public string OrderId { get; }
        public string OrderInfo { get; }
        public string RedirectUrl { get; }
        public string IpnUrl { get; }
        public string RequestType { get; }
        public string ExtraData { get; }
        public string Lang { get; }
        public string Signature { get; private set; }

        public void MakeSignature(string accessKey, string secretKey)
        {
            var rawHash = $"accessKey={accessKey}&amount={Amount}&extraData={ExtraData}&ipnUrl={IpnUrl}&orderId={OrderId}&orderInfo={OrderInfo}&partnerCode={PartnerCode}&redirectUrl={RedirectUrl}&requestId={RequestId}&requestType={RequestType}";
            Signature = HashHelper.HmacSHA256(rawHash, secretKey);
        }

        public (bool, string?) GetLink(string paymentUrl)
        {
            using var client = new HttpClient();

            try
            {
                var requestData = JsonConvert.SerializeObject(this, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                var requestContent = new StringContent(requestData, Encoding.UTF8, "application/json");
                var createPaymentLinkRes = client.PostAsync(paymentUrl, requestContent).Result;

                if (createPaymentLinkRes.IsSuccessStatusCode)
                {
                    var responseContent = createPaymentLinkRes.Content.ReadAsStringAsync().Result;
                    var responseData = JsonConvert.DeserializeObject<MomoOneTimePaymentCreateLinkResponse>(responseContent);
                    if (responseData.resultCode == "0")
                    {
                        return (true, responseData.payUrl);
                    }
                    else
                    {
                        return (false, responseData.message);
                    }
                }
                else
                {
                    return (false, createPaymentLinkRes.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
