using Core.Common;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace Services.Services
{
    public class SMSService: ISMSService
    {
        private readonly IOptions<SMSMsegat> config;
        readonly Uri baseAddress;
        public SMSService(IOptions<SMSMsegat> config)
        {
            this.config = config;
            baseAddress = new Uri(config.Value.Uri);
        }
        public async Task<SmsMsegatResponseBody> SendAsync(string message, string PhoneNumber)
        {
            try
            {

            
            PhoneNumber = PhoneNumber.Replace("+", "");
            long phoneNumber = long.Parse(PhoneNumber);
            PhoneNumber = phoneNumber.ToString();
            SmsMsegatRequestBody msgBody = new SmsMsegatRequestBody();
            msgBody.userSender = config.Value.UserSender;
            msgBody.userName = config.Value.UserName;
            msgBody.apiKey = config.Value.ApiKey;
            msgBody.msg = message;
            msgBody.numbers = PhoneNumber;
            SmsMsegatResponseBody responseBody = new SmsMsegatResponseBody();
            using var httpClient = new HttpClient { BaseAddress = baseAddress };
            {
                using (var content = new StringContent(JsonConvert.SerializeObject(msgBody)
                                    , System.Text.Encoding.Default, "application/json"))
                {
                    using (var response = await httpClient.PostAsync("/gw/sendsms.php", content))
                    {
                        string responseHeaders = response.Headers.ToString();
                        string responseData = await response.Content.ReadAsStringAsync();
                        responseBody.Status = (int)response.StatusCode;
                        responseBody.Headers = responseHeaders;
                        responseBody.Data = responseData;
                    }
                }
            }
            return responseBody;
            }
            catch (Exception ex)
            {
                return new SmsMsegatResponseBody() {  Status= 400 , Data="failed to send sms"};
            }
        }
    }
}
