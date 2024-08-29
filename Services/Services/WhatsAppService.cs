using Core.Common;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;

namespace Services.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IOptions<NexmoSettings> config;
        readonly Uri baseAddress;
        public WhatsAppService(IOptions<NexmoSettings> config)
        {
            this.config = config;
            baseAddress = new Uri(config.Value.uri);
        }
        public async Task<WhatsAppResponseBody> SendAsync(string message, string PhoneNumber)
        {
            try
            {
                PhoneNumber = PhoneNumber.Replace("+", "");
                long phoneNumber = long.Parse(PhoneNumber);
                PhoneNumber = phoneNumber.ToString();
                WhatsAppRequestBody msgBody = new WhatsAppRequestBody();
                msgBody.text = message;
                msgBody.to = PhoneNumber;
                msgBody.message_type = config.Value.message_type;
                msgBody.from = config.Value.from;
                msgBody.channel = config.Value.channel;
                var authenticationString = $"{config.Value.clientId}:{config.Value.clientSecret}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
                WhatsAppRequestBody responseBody = new WhatsAppRequestBody();
                using var httpClient = new HttpClient { BaseAddress = baseAddress };
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(config.Value.clientId + ":" + config.Value.clientSecret)));
                    using (var content = new StringContent(JsonConvert.SerializeObject(msgBody), Encoding.Default, "application/json"))
                    {
                        using (var response = await httpClient.PostAsync("/v1/messages", content))
                        {
                            string responseHeaders = response.Headers.ToString();
                            string responseData = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                return new WhatsAppResponseBody() { Status = 200, Data = "Succeeded" };
            }
            catch (Exception ex)
            {
                return new WhatsAppResponseBody() { Status = 400, Data = "failed to send sms" };
            }
        }
    }
}
